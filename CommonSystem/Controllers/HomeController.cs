using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Diagnostics;
using ApplicationLayer.IService;
using CommonSystem.Filter;
using Domain.Enums;
using System;
using ApplicationLayer.EntityDto.BaseDto;
using Infrastructrue;
using CommonSystem.ModelHelper;

namespace CommonSystem.Controllers
{
    /// <summary>
    /// 首页
    /// </summary>
    [IgnoreActionFilterAttribute]
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IUserService _userService;
        private readonly IMenuService _menuService;
        private readonly IMessageService _messageService;
        private readonly IWebHostEnvironment _hostEnvironment;

        public HomeController(IUserService userSvr, 
            IMenuService menuService,
            IMessageService messageService,
            IWebHostEnvironment hostEnvironment)
        {
            _userService = userSvr;
            _menuService = menuService;
            _hostEnvironment = hostEnvironment;
            _messageService = messageService;
            //log.Error("home ctor error");
        }

        /// <summary>
        /// 首页
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            var userId = User.Identity.GetLoginUserId();
            var myMenus = await _menuService.GetMyMenusAsync(userId);
            var myUnReadMessageNumber = await _messageService.GetMyMessageCountAsync(userId);
            var myUnReadMessages = await _messageService.GetUnReadMesasgeAsync(userId);
            ViewBag.Menus = myMenus;
            ViewBag.MyUnReadMessageNumber = myUnReadMessageNumber;
            return View(myUnReadMessages);
        }

        /// <summary>
        /// 欢迎页面
        /// </summary>
        /// <returns></returns>
        public IActionResult Welcome()
        {
            return View();
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public IActionResult Login()
        {

            var model = new LoginDto
            {
                ReturnUrl = Request.Query["ReturnUrl"],
                LoginName = "admin",
                Password = "qwaszx"
            };
            return View(model);
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(LoginDto model)
        {

            if (!ModelState.IsValid) return View(model);

            var connection = Request.HttpContext.Features.Get<IHttpConnectionFeature>();

            //var localIpAddress = connection.LocalIpAddress;    //本地IP地址
            //var localPort = connection.LocalPort;              //本地IP端口
            var remoteIpAddress = connection.RemoteIpAddress;  //远程IP地址
            //var remotePort = connection.RemotePort;            //本地IP端口

            model.LoginIP = remoteIpAddress.ToString();
            var loginDto = await _userService.LoginAsync(model);
            if (loginDto.LoginSuccess)
            {
                var authenType = CookieAuthenticationDefaults.AuthenticationScheme;
                var identity = new ClaimsIdentity(authenType);
                identity.AddClaim(new Claim(ClaimTypes.Name, loginDto.User.LoginName));
                //原存储在改为Session
                identity.AddClaim(new Claim("LoginUserId", loginDto.User.Id.ToString()));
                MyHttpContext.Current.setSession("UserId", loginDto.User.Id.ToString());
                var properties = new AuthenticationProperties() { IsPersistent = true };
                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(authenType, principal, properties);
                model.ReturnUrl = model.ReturnUrl.IsNotBlank() ? model.ReturnUrl : "/";
                return Redirect(model.ReturnUrl);
            }
            ModelState.AddModelError(loginDto.Result == LoginResult.AccountNotExists ? "LoginName" : "Password",
                loginDto.Message);
            return View(model);
        }

        /// <summary>
        /// 退出登录
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        /// <summary>
        /// 错误页面
        /// </summary>
        /// <returns></returns>
        [IgnoreActionFilterAttribute]
        [AllowAnonymous]
        public IActionResult Error()
        {
            var feature = HttpContext.Features.Get<IExceptionHandlerFeature>();
            var error = feature?.Error;
            if (error != null)
            {
                //Log.Logger.Error(error);
            }
            var isAjax = false;
            var xreq = Request.Headers.ContainsKey("x-requested-with");
            if (xreq)
            {
                isAjax = Request.Headers["x-requested-with"] == "XMLHttpRequest";
            }
            if (isAjax)
            {
                if(error is Exception)
                {
                    Response.StatusCode = 200;
                }
                return Json(new JsonResult<string>(false, error?.Message, string.Empty));
            }
            return View();
        }
    }
}