using System;
using System.Linq;
using System.Net;
using ApplicationLayer.IService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace CommonSystem.Filter
{
    public class ValidateFilter : ActionFilterAttribute
    {
        /// <summary>
        /// 权限验证
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            //当加上了忽略属性时
            var IsIgnored = context.ActionDescriptor.FilterDescriptors.Any(f => f.Filter is IgnoreActionFilterAttribute);
            if (IsIgnored) return;

            var userService = context.HttpContext.RequestServices.GetService<IUserService>();
            var identity = context.HttpContext.User.Identity;
            var route = context.RouteData.Values;
            var controller = route["controller"];
            var actionName = route["action"];
            string url = string.Format("/{0}/{1}", controller, actionName);
            var validate = userService.HasRightAsync(identity.GetLoginUserId(), url).Result;

            if (validate) return;

            //是否为Ajax请求
            //可以看到 Ajax 请求多了个 x-requested - with ，可以利用它，
            //request.getHeader(“x - requested - with”); 为 null，则为传统同步请求，
            //为 XMLHttpRequest，则为 Ajax 异步请求。
            var isAjax = context.HttpContext.Request.Headers["X-Requested-With"].ToString()
                .Equals("XMLHttpRequest", StringComparison.OrdinalIgnoreCase);

            IActionResult action;
            if (isAjax)
            {
                var data = new
                {
                    flag=false,
                    code=HttpStatusCode.Unauthorized,
                    msg="您没有权限访问！"
                };
                action = new JsonResult(data);
            }
            else
            {
                action = new ViewResult
                {
                    ViewName="~/Views/Shared/NoValidate.cshtml"
                };
            }

            context.Result = action;

        }
    }
}
