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
                    //Cookie��Чʱ��
                    cookie.ExpireTimeSpan = TimeSpan.FromMinutes(360);
                    //��Ȩ��֤ûͨ������е�¼��֤
                    cookie.LoginPath = new Microsoft.AspNetCore.Http.PathString("/Home/Login");
                    //�ǳ� ���������Cookie
                    cookie.LogoutPath = new Microsoft.AspNetCore.Http.PathString("/Home/Logout");
                    //�������ڴ���Cookie������
                    cookie.Cookie = new Microsoft.AspNetCore.Http.CookieBuilder()
                    {
                        HttpOnly = true,
                        Name = "CommonSystem-Identity",
                        Path = "/"
                    };
                });
            //����sql
            string ConStr = Configuration.GetConnectionString("SqlServer").ToString();
            //services.AddDbContext<DBContext>(option=>option.UseSqlServer(�����ַ���));
            //��������������ݿ�
            
            services.AddHangfire(hang=>hang.UseSqlServerStorage(ConStr));
            //Ȩ����֤filter    �����
            //��������ע������
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
            //��̬�ļ�
            app.UseStaticFiles();
            //��֤
            app.UseAuthentication();
            //·��
            app.UseRouting();
            //��Ȩ
            app.UseAuthorization();
          
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(name: "Default", pattern: "{Controller=Home}/{Action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
            #warning  �����
            //��¼���� ��¼Middleware  �����
            //��ʼ�����ݿ⼰��ʼ����

            //ʹ��HangFire�Զ����������
            var HangJobOption = new BackgroundJobServerOptions()
            {
                //����������
                ServerName = Environment.MachineName,
                //�������
                WorkerCount = 3,
                //ִ���������
                Queues = new string[] {"default","api" }
            };
            app.UseHangfireServer(HangJobOption);
            //����HangFire�������Ȩ����֤
            var HangJobAuth = new BasicAuthAuthorizationFilter(
                new BasicAuthAuthorizationFilterOptions
                {
                    //�����ض���
                    SslRedirect = false,
                    //��ҪSsl  Https
                    RequireSsl = false,
                    //���Դ�Сд
                    LoginCaseSensitive = false,
                    Users = new[]
                    {
                        //�����˺�����
                        new BasicAuthAuthorizationUser
                        {
                            Login="admin",
                            Password=Encoding.UTF8.GetBytes("CommonSystem"),
                            PasswordClear="CommonSystem"
                        }
                    }
                }
            );
            //�ı�ԭ�����ʺ�̨����·��
            app.UseHangfireDashboard("/TaskManager", new DashboardOptions
            {
                Authorization = new[]
                {
                    HangJobAuth
                }
            });
            //���һ��ÿ���Զ����賿��ʱ��ִ�е�ͳ������    ������
            //RecurringJob.AddOrUpdate<ISiteViewService>(x => x.AddOrUpdate(), Cron.Daily());
            //RecurringJob.AddOrUpdate(() => Console.WriteLine($"Job��{DateTime.Now}ִ�����."), Cron.Minutely());
        }
    }
}
