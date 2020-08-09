using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommonSystem.ModelHelper;
using Infrastructrue;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CommonSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        


        public IActionResult Index()
        {
            LoggerHelper.LogError("111111111111111111111111111111111222222222222222222222222222222");
            return View();
        }

        public IActionResult Test()
        {
            string Code = SecurityExtendHelper.CreateValidateCode(4);
            byte[] bytes =HttpContext.CreateValidateGraphic(Code);
            return File(bytes,@"image/jpeg");
        }
    }
}
