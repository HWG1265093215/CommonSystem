using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationLayer.EntityDto.BaseDto;
using ApplicationLayer.Filters;
using ApplicationLayer.IService;
using CommonSystem.Filter;
using CommonSystem.ModelHelper;
using Dapper;
using Infrastructrue;
using Microsoft.AspNetCore.Mvc;

namespace CommonSystem.Controllers
{
    [IgnoreActionFilterAttribute]
    public class DemoController : Controller
    {
        public IModelTempService _tempService { get; set; }

        [Meun(Id =Menu.DemoPageId,ParentId =Menu.SystemId,Order ="10",Name ="模板配置页面")]
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetListWithPager(BaseFilter baseFilter)
        {
            var ListAll = await _tempService.SearchAsync(baseFilter);
            return Json(ListAll); ;
        }

        public IActionResult GetSqlTableDetails(string TableName)
        {
            DynamicParameters dy = new DynamicParameters();
            dy.Add("TableName","ModelTemp");
            var TableDetails = DapperHelper.ExcuteProduce("TableDetails",dy);
            return Json(TableDetails);
        }

        public ActionResult Edit()
        {
            return View(new ModelEntityDto());
        }

        public IActionResult EditModel(ModelEntityDto entityDto)
        {
            return Content("1");
        }
    }
}
