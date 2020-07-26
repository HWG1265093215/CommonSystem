using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommonSystem.Filter;
using Infrastructrue;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CommonSystem.Controllers
{
    public class HomeController : Controller
    {
         
        private readonly ILogger<HomeController> _logger;

        private readonly IHostEnvironment _environment;
        public HomeController(ILogger<HomeController> logger,IHostEnvironment environment)
        {
            _logger = logger;
            _environment = environment;
        }
        public IActionResult Index()
        {
           
            this.HttpContext.Response.Clear();

            string filePath = ControllerExtension.GetabsoluteFilePath(@"~\Content\捕获1.PNG");

            string tempPath = ControllerExtension.GetTempFilePath(@"~\Content\捕获1.PNG");

            FileStream file = new FileStream(filePath,FileMode.OpenOrCreate,FileAccess.Read);

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
