using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommonSystem.Filter;
using Microsoft.AspNetCore.Mvc;

namespace CommonSystem.Controllers
{
    [IgnoreActionFilterAttribute]
    public class JobController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
