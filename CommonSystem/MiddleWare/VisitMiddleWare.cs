using ApplicationLayer.IService;
using CommonSystem.ModelHelper;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http.Connections.Features;
using ApplicationLayer.EntityDto.BaseDto;
using Microsoft.AspNetCore.Http.Features;

namespace CommonSystem.MiddleWare
{
    public class VisitMiddleWare
    {
        private RequestDelegate _request;

        public VisitMiddleWare(RequestDelegate request)
        {
            _request = request;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                var userService = context.RequestServices.GetService<IUserService>();
                //获取Http特性集合
                var connection = context.Features.Get<IHttpConnectionFeature>();
                var user = context.User;
                var isLogined = user?.Identity!=null && user.Identity.IsAuthenticated;

                var visit = new VisitDto
                {
                    Ip=connection.RemoteIpAddress.ToString(),
                    LoginName=isLogined?user.Identity.Name:string.Empty,
                    Url=context.Request.Path,
                    UserId=user.Identity.GetLoginUserId()
                };
                await userService.VisitAsync(visit);
            }
            catch (Exception e)
            {
                LoggerHelper.LogError(e.Message);
            }
        }
    }
}
