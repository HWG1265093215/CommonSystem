using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.Dashboard.BasicAuthorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

namespace CommonSystem
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

        }

        public IConfiguration Configuration { get; }
        public ILoggerFactory _loggerFactory { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(cookie =>
                {
                    //Cookie有效时间
                    cookie.ExpireTimeSpan = TimeSpan.FromMinutes(360);
                    //授权验证没通过则进行登录验证
                    cookie.LoginPath = new Microsoft.AspNetCore.Http.PathString("/Home/Login");
                    //登出 类似于清空Cookie
                    cookie.LogoutPath = new Microsoft.AspNetCore.Http.PathString("/Home/Logout");
                    //定义用于创建Cookie的设置
                    cookie.Cookie = new Microsoft.AspNetCore.Http.CookieBuilder()
                    {
                        HttpOnly = true,
                        Name = "CommonSystem-Identity",
                        Path = "/"
                    };
                });
            //连接sql
            string ConStr = Configuration.GetConnectionString("SqlServer").ToString();
            //services.AddDbContext<DBContext>(option=>option.UseSqlServer(连接字符串));
            //任务调度连接数据库
            
            services.AddHangfire(hang=>hang.UseSqlServerStorage(ConStr));
            //权限验证filter    待添加
            //批量依赖注入待添加
            services.AddRazorPages();
            services.AddMvc();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            loggerFactory.AddNLog();
            //静态文件
            app.UseStaticFiles();
            //认证
            app.UseAuthentication();
            //路由
            app.UseRouting();
            //授权
            app.UseAuthorization();
          
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(name: "Default", pattern: "{Controller=Home}/{Action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
            #warning  待添加
            //记录访问 记录Middleware  待添加
            //初始化数据库及初始数据

            //使用HangFire自动化任务调度
            var HangJobOption = new BackgroundJobServerOptions()
            {
                //服务器名字
                ServerName = Environment.MachineName,
                //最大并行数
                WorkerCount = 3,
                //执行任务队列
                Queues = new string[] {"default","api" }
            };
            app.UseHangfireServer(HangJobOption);
            //设置HangFire控制面板权限验证
            var HangJobAuth = new BasicAuthAuthorizationFilter(
                new BasicAuthAuthorizationFilterOptions
                {
                    //加密重定向
                    SslRedirect = false,
                    //需要Ssl  Https
                    RequireSsl = false,
                    //忽略大小写
                    LoginCaseSensitive = false,
                    Users = new[]
                    {
                        //配置账号密码
                        new BasicAuthAuthorizationUser
                        {
                            Login="admin",
                            Password=Encoding.UTF8.GetBytes("CommonSystem"),
                            PasswordClear="CommonSystem"
                        }
                    }
                }
            );
            //改变原本访问后台任务路径
            app.UseHangfireDashboard("/TaskManager", new DashboardOptions
            {
                Authorization = new[]
                {
                    HangJobAuth
                }
            });
            //添加一个每天自动在凌晨的时候执行的统计任务    待完善
            //RecurringJob.AddOrUpdate<ISiteViewService>(x => x.AddOrUpdate(), Cron.Daily());
            //RecurringJob.AddOrUpdate(() => Console.WriteLine($"Job在{DateTime.Now}执行完成."), Cron.Minutely());
        }
    }
}
