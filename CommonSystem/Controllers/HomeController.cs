using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
           
            this.HttpContext.Response.Clear();
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
