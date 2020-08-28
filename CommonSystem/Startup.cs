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
            string conStr = Configuration.GetConnectionString("SqlServer").ToString();
            services.AddDbContext<DBContext>(option=>option.UseSqlServer(conStr));

            services.AddParamterConstruct();

            services.AddSession();

            //��������������ݿ�
            services.AddHangfire(hang=>hang.UseSqlServerStorage(conStr));
            //��������ע������
            services.AddRazorPages();
            //�Ƴ�������ͼ����
            services.Configure<RazorViewEngineOptions>(option=>
            {
                //{2} ������������   {1}����������  {0}��ͼ����
                option.AreaViewLocationFormats.Clear();
                option.AreaViewLocationFormats.Add("Area/{2}/Views/{1}/{0}.cshtml");
                option.AreaViewLocationFormats.Add("Area/{2}/Views/Shared/{0}.cshtml");
            });
            services.AddScoped<IHttpContextAccessor, HttpContextAccessor>();
            //Ȩ����֤filter
            services.AddMvc(cfg=>cfg.Filters.Add(new ValidateFilter()));
            services.Replace(ServiceDescriptor.Transient<IControllerActivator, ServiceBasedControllerActivator>());

        }

        //https://autofaccn.readthedocs.io/zh/latest/integration/aspnetcore.html  AutoFac Api�ĵ�
        //�����淶   �ȿ�
        public void ConfigureContainer(ContainerBuilder  builder)
        {
            //ע��Aop �������   ������
            //builder.RegisterType<CustomAutoFacAopInterception>();
            //��ȡ����ע����
            var injectDenpency = Assembly.Load("ApplicationLayer");
            //���ӿڶ�ʵ��   ������
            
            //��ȡ��Ӧ����
            builder.RegisterAssemblyTypes(injectDenpency)
                .Where(inject => inject.Namespace.EndsWith("IServiceImpl", StringComparison.OrdinalIgnoreCase) && inject.GetInterfaces().Length > 0).AsImplementedInterfaces().PropertiesAutowired();
            //��������ע��
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
            //��¼���� ��¼Middleware
            app.UseMiddleware<VisitMiddleWare>();
            //��ʼ�����ݿ⼰��ʼ����
            Task.Run(async()=>
            {
                using (var scope = app.ApplicationServices.CreateScope())
                {
                    var meuns = MeunHelper.GetMeunes();
                    var dbservice = scope.ServiceProvider.GetService<IDatabaseInitService>();
                    await dbservice.InitAsync(meuns);
                }
            });
            //ʹ��HangFire�Զ����������
            var hangJobOption = new BackgroundJobServerOptions()
            {
                //����������
                ServerName = Environment.MachineName,
                //�������
                WorkerCount = 3,
                //ִ���������
                Queues = new string[] {"default","api" }
            };
            app.UseHangfireServer(hangJobOption);
            //����HangFire�������Ȩ����֤
            var hangJobAuth = new BasicAuthAuthorizationFilter(
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
                    hangJobAuth
                }
            });
            //���һ��ÿ���Զ����賿��ʱ��ִ�е�ͳ������    ������
            RecurringJob.AddOrUpdate<ISiteViewService>(x => x.AddOrUpdate(), Cron.Daily());
            RecurringJob.AddOrUpdate(() => Console.WriteLine($"Job��{DateTime.Now}ִ�����."), Cron.Minutely());
        }
    }

    public static class IdentityExtention
    {
        public static string GetLoginUserId(this IIdentity identity)
        {
            var claim = (identity as ClaimsIdentity)?.FindFirst("LoginUserId");

            return claim == null ? string.Empty : claim.Value;
        }

        //ע�������ʵ�����캯������
        public static void AddParamterConstruct(this IServiceCollection  services)
        {
            //ISet<Type> listType = new HashSet<Type>();
            //var temp = Assembly.Load("ApplicationLayer");
            ////��ȡʵ����
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
            //����AutoMapperӳ���ϵ
            services.AddScoped<AutoMapper.IConfigurationProvider>(_=>AutoMapperConfig.GetMapperConfiguration());
            services.AddScoped(_ => AutoMapperConfig.GetMapperConfiguration().CreateMapper());

        }
    }
}
