using ApplicationLayer.EntityDto.BaseDto;
using ApplicationLayer.IService;
using CommonSystem.ModelHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CommonSystem.Controllers
{
    /// <summary>
    /// 系统管理
    /// </summary>
    [Authorize]
    public class SystemController : Controller
    {
        private readonly IDatabaseInitService _databaseInitService;
        private readonly IMenuService _menuService;

        public SystemController(IDatabaseInitService databaseInitService, IMenuService menuService)
        {
            _databaseInitService = databaseInitService;
            _menuService = menuService;
        }

        /// <summary>
        /// 系统管理首页
        /// </summary>
        /// <returns></returns>
        [MeunAttribute(Id = Menu.SystemConfigId, ParentId = Menu.SystemId, Name = "系统配置", Order = "1")]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 重置路径码
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [MeunAttribute(Id = Menu.ResetPathCodeId, ParentId = Menu.SystemConfigId, Name = "重置路径码", Order = "1")]
        public async Task<IActionResult> ReloadPathCode()
        {
            var result = new JsonResult<bool>
            {
                flag = await _databaseInitService.InitPathCodeAsync()
            };
            return Json(result);
        }

        /// <summary>
        /// 初始化省市区数据
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [MeunAttribute(Id = Menu.ResetAreaId, ParentId = Menu.SystemConfigId, Name = "重置省市区基础数据", Order = "2")]
        public async Task<JsonResult> ReInitAreas()
        {
            var result = new JsonResult<bool>
            {
                flag = await _databaseInitService.InitAreas()
            };
            return Json(result);
        }

        /// <summary>
        /// 重置系统所有菜单
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ReInitMenues()  
        {
            var menues = MeunHelper.GetMeunes();
            var succes = await _menuService.ReInitMenuesAsync(menues);
            var result = new JsonResult<bool> { flag = succes };
            return Json(result);
        }
    }
}