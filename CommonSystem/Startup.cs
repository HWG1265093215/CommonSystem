using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using ApplicationLayer.IService;
using ApplicationLayer.IServiceImpl;
using Autofac;
using AutoMapper;
using CommonSystem.Filter;
using CommonSystem.MiddleWare;
using CommonSystem.ModelHelper;
using Domain;
using Hangfire;
using Hangfire.Dashboard.BasicAuthorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using NLog.Web;

namespace CommonSystem
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

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
            string conStr = Configuration.GetConnectionString("SqlServer").ToString();
            services.AddDbContext<DBContext>(option=>option.UseSqlServer(conStr));

            services.AddParamterConstruct();

            services.AddSession();

            //任务调度连接数据库
            services.AddHangfire(hang=>hang.UseSqlServerStorage(conStr));
            //批量依赖注入待添加
            services.AddRazorPages();
            //移除多余视图引擎
            services.Configure<RazorViewEngineOptions>(option=>
            {
                //{2} 代表区域名称   {1}控制器名称  {0}视图名称
                option.AreaViewLocationFormats.Clear();
                option.AreaViewLocationFormats.Add("Area/{2}/Views/{1}/{0}.cshtml");
                option.AreaViewLocationFormats.Add("Area/{2}/Views/Shared/{0}.cshtml");
            });
            services.AddScoped<IHttpContextAccessor, HttpContextAccessor>();
            //权限验证filter
            services.AddMvc(cfg=>cfg.Filters.Add(new ValidateFilter()));
            services.Replace(ServiceDescriptor.Transient<IControllerActivator, ServiceBasedControllerActivator>());

        }

        //https://autofaccn.readthedocs.io/zh/latest/integration/aspnetcore.html  AutoFac Api文档
        //命名规范   踩坑
        public void ConfigureContainer(ContainerBuilder  builder)
        {
            //注册Aop 添加切面   待完善
            //builder.RegisterType<CustomAutoFacAopInterception>();
            //获取依赖注入类
            var injectDenpency = Assembly.Load("ApplicationLayer");
            //单接口多实现   待完善
            
            //获取对应程序集
            builder.RegisterAssemblyTypes(injectDenpency)
                .Where(inject => inject.Namespace.EndsWith("IServiceImpl", StringComparison.OrdinalIgnoreCase) && inject.GetInterfaces().Length > 0).AsImplementedInterfaces().PropertiesAutowired();
            //设置属性注入
            builder.RegisterModule<DefaultModule>();
            int a = builder.Properties.Count;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.GetServiceProvider();
            app.UseSession();
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
            //记录访问 记录Middleware
            app.UseMiddleware<VisitMiddleWare>();
            //初始化数据库及初始数据
            Task.Run(async()=>
            {
                using (var scope = app.ApplicationServices.CreateScope())
                {
                    var meuns = MeunHelper.GetMeunes();
                    var dbservice = scope.ServiceProvider.GetService<IDatabaseInitService>();
                    await dbservice.InitAsync(meuns);
                }
            });
            //使用HangFire自动化任务调度
            var hangJobOption = new BackgroundJobServerOptions()
            {
                //服务器名字
                ServerName = Environment.MachineName,
                //最大并行数
                WorkerCount = 3,
                //执行任务队列
                Queues = new string[] {"default","api" }
            };
            app.UseHangfireServer(hangJobOption);
            //设置HangFire控制面板权限验证
            var hangJobAuth = new BasicAuthAuthorizationFilter(
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
                    hangJobAuth
                }
            });
            //添加一个每天自动在凌晨的时候执行的统计任务    待完善
            RecurringJob.AddOrUpdate<ISiteViewService>(x => x.AddOrUpdate(), Cron.Daily());
            RecurringJob.AddOrUpdate(() => Console.WriteLine($"Job在{DateTime.Now}执行完成."), Cron.Minutely());
        }
    }

    public static class IdentityExtention
    {
        public static string GetLoginUserId(this IIdentity identity)
        {
            var claim = (identity as ClaimsIdentity)?.FindFirst("LoginUserId");

            return claim == null ? string.Empty : claim.Value;
        }

        //注册服务器实例构造函数参数
        public static void AddParamterConstruct(this IServiceCollection  services)
        {
            //ISet<Type> listType = new HashSet<Type>();
            //var temp = Assembly.Load("ApplicationLayer");
            ////获取实现类
            //var injectDenpency = Assembly.Load("ApplicationLayer").GetTypes().Where(n => n.FullName.Contains("IServiceImpl") && !string.IsNullOrEmpty(n.FullName));

            //foreach (var type in injectDenpency)
            //{
            //    foreach (var con in type.GetConstructors())
            //    {
            //        foreach (var parameter in con.GetParameters())
            //        {
            //            listType.Add(parameter.ParameterType);
            //        }
            //    }
            //}

            //foreach (var list in listType)
            //{
            //    var types = AppDomain.CurrentDomain.GetAssemblies()
            //        .SelectMany(a => a.GetTypes().Where(t => t.GetInterfaces().Contains(list)))
            //        .ToArray();
            //    foreach (var impl in types)
            //    {
            //        services.AddScoped(list, impl);
            //    }
            //}

            services.AddScoped<DbContext,DBContext>();
            //加载AutoMapper映射关系
            services.AddScoped<AutoMapper.IConfigurationProvider>(_=>AutoMapperConfig.GetMapperConfiguration());
            services.AddScoped(_ => AutoMapperConfig.GetMapperConfiguration().CreateMapper());

        }
    }
}
