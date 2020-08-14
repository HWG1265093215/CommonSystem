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
    public class Home1Controller : Controller
    {
        private readonly ILogger<Home1Controller> _logger;
        public Home1Controller(ILogger<Home1Controller> logger)
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
