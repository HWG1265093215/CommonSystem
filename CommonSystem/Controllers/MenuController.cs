using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using ApplicationLayer.IService;
using CommonSystem.ModelHelper;
using ApplicationLayer.EntityDto.BaseDto;
using Domain.Meun;
using CommonSystem.Filter;
using Infrastructrue;
using ApplicationLayer.Filters;

namespace CommonSystem.Controllers
{
    /// <summary>
    /// 菜单
    /// </summary>
    [Authorize]
    public class MenuController : Controller
    {
        private readonly IMenuService _menuService;

        public MenuController(IMenuService menuSvc)
        {
            _menuService = menuSvc;
        }

        /// <summary>
        /// 首页
        /// </summary>
        /// <returns></returns>
        [MeunAttribute(Id = Menu.MenuPageId, ParentId = Menu.SystemId, Name = "菜单管理", Order = "4")]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <returns></returns>
        [MeunAttribute(Id = Menu.MenuAddId, ParentId = Menu.MenuPageId, Name = "添加菜单", Order = "1")]
        public IActionResult Add()
        {
            SetTypeList(MenuType.Module);
            return View(new MenuDto());
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [MeunAttribute(Id = Menu.MenuEditId, ParentId = Menu.MenuPageId, Name = "编辑菜单", Order = "2")]
        public async Task<IActionResult> Edit(string id)
        {
            var model = await _menuService.FindAsync(id);
            SetTypeList(model.Type);
            return View(model);
        }

        /// <summary>
        /// 图标选择
        /// </summary>
        [IgnoreActionFilterAttribute]
        public IActionResult FontAwesome()
        {
            return View();
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Add(MenuDto dto)
        {
            SetTypeList(dto.Type);
            if (ModelState.IsValid)
            {
                var result = await _menuService.AddAsync(dto);
                if (result.IsNotBlank())
                    return RedirectToAction("Index");
            }
            return View(dto);
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Edit(MenuDto dto)
        {
            SetTypeList(dto.Type);
            if (ModelState.IsValid)
            {
                var result = await _menuService.UpdateAsync(dto);
                if (result)
                    return RedirectToAction("Index");
            }
            return View(dto);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [MeunAttribute(Id = Menu.MenuDeleteId, ParentId = Menu.MenuPageId, Name = "删除菜单", Order = "3")]
        public async Task<IActionResult> Delete([FromBody]IEnumerable<string> ids)
        {
            var result = new JsonResult<bool>();
            if (ids.AnyOne())
            {
                result.flag = await _menuService.DeleteAsync(ids);
            }
            return Json(result);
        }

        /// <summary>
        /// 搜索页面
        /// </summary>
        /// <param name="filters">查询参数</param>
        /// <returns></returns>
        [IgnoreActionFilterAttribute]
        public async Task<IActionResult> GetListWithPager(MenuFilters filters)
        {
            var result = await _menuService.SearchAsync(filters);
            return Json(result);
        }

        /// <summary>
        /// 搜索页面
        /// </summary>
        /// <param name="filters">查询参数</param>
        /// <returns></returns>
        [IgnoreActionFilterAttribute]
        public async Task<IActionResult> GetListWithKeywords(MenuFilters filters)
        {
            filters.page = 1;
            filters.rows = 10;
            filters.ExcludeType = MenuType.Action; 
            var result = await _menuService.SearchAsync(filters);
            return Json(new {value = result.rows});
        }

        private void SetTypeList(MenuType type)
        {
            var typeList = new List<SelectListItem>
            {
                new SelectListItem(MenuType.Module.GetDescriptionForEnumName(),((int)MenuType.Module).ToString(), type == MenuType.Module),
                new SelectListItem(MenuType.Menu.GetDescriptionForEnumName(),((int)MenuType.Menu).ToString(), type == MenuType.Menu),
                new SelectListItem(MenuType.Action.GetDescriptionForEnumName(),((int)MenuType.Action).ToString(), type == MenuType.Action),
            };
            ViewBag.TypeList = typeList;
        }
    }
}