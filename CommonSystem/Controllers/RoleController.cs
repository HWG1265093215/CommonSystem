using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System;
using ApplicationLayer.IService;
using CommonSystem.ModelHelper;
using ApplicationLayer.EntityDto.BaseDto;
using Infrastructrue;
using CommonSystem.Filter;
using ApplicationLayer.Filters;

namespace CommonSystem.Controllers
{
    /// <summary>
    /// 用户角色
    /// </summary>
    [Authorize]
    public class RoleController : Controller
    {
        private readonly IRoleService _roleService;
        private readonly IMenuService _menuService;

        public RoleController(IRoleService roleSvc,IMenuService menuService)
        {
            _roleService = roleSvc;
            _menuService = menuService;
        }

        /// <summary>
        /// 首页
        /// </summary>
        /// <returns></returns>
        [MeunAttribute(Id = Menu.RolePageId, ParentId = Menu.SystemId, Name = "角色管理", Order = "5")]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <returns></returns>
        [MeunAttribute(Id = Menu.RoleAddId, ParentId = Menu.RolePageId, Name = "添加角色", Order = "1")]
        public IActionResult Add()
        {
            return View(new RoleDto());
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [MeunAttribute(Id = Menu.RoleEditId, ParentId = Menu.RolePageId, Name = "编辑角色", Order = "2")]
        public async Task<IActionResult> Edit(string id)
        {
            var model = await _roleService.FindAsync(id);
            return View(model);
        }

        /// <summary>
        /// 角色授权
        /// </summary>
        /// <returns></returns>
        [MeunAttribute(Id = Menu.RoleAuthorizeId, ParentId = Menu.SystemId, Name = "角色授权", Order = "6")]
        public IActionResult Authen()
        {
            return View();
        }

        /// <summary>
        /// 获取菜单树
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> AuthenMenuDatas()
        {
            var list = await _menuService.GetTreesAsync();
            //默认tree节点是展开的
            list.ForEach(x => x.open = true);
            return Json(list);
        }

        /// <summary>
        /// 获取角色树
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> AuthenRoleDatas()
        {
            var list = await _roleService.GetTreesAsync();
            return Json(list);
        }

        /// <summary>
        /// 获取角色下的菜单
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> AuthenRoleMenus(string id)
        {
            var list = await _menuService.GetMenusByRoleIdAsync(id);
            var menuIds = list?.Select(item => item.Id);
            return Json(menuIds);
        }

        /// <summary>
        /// 设置角色的权限
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [MeunAttribute(Id = Menu.RoleSetAuthorizeId, ParentId = Menu.RoleAuthorizeId, Name = "设置角色权限", Order = "1")]
        public async Task<IActionResult> SetRoleMenus([FromBody]List<RoleMenuDto> datas)
        {
            var result = new JsonResult<bool>();
            if (datas.AnyOne())
            {
                result.flag = await _roleService.SetRoleMenusAsync(datas);
            }
            else
            {
                result.msg = "请选择权限";
            }
            return Json(result);
        }

        /// <summary>
        /// 清空该角色下的所有权限
        /// </summary>
        /// <param name="id">角色ID</param>
        /// <returns></returns>
        [HttpPost]
        [MeunAttribute(Id = Menu.RoleCancelAuthorizeId, ParentId = Menu.RoleAuthorizeId, Name = "清空权限", Order = "2")]
        public async Task<IActionResult> ClearRoleMenus(string id)
        {
            var result = new JsonResult<bool>
            {
                flag = await _roleService.ClearRoleMenusAsync(id)
            };
            return Json(result);
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Add(RoleDto dto)
        {
            if (ModelState.IsValid)
            {
                var result = await _roleService.AddAsync(dto);
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
        public async Task<IActionResult> Edit(RoleDto dto)
        {
            if (!ModelState.IsValid) return View(dto);

            var result = await _roleService.UpdateAsync(dto);
            if (result)
                return RedirectToAction("Index");
            return View(dto);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [MeunAttribute(Id = Menu.RoleDeleteId, ParentId = Menu.RolePageId, Name = "删除角色", Order = "3")]
        public async Task<IActionResult> Delete([FromBody]IEnumerable<string> ids)
        {
            var result = new JsonResult<bool>();
            if (ids.AnyOne())
            {
                result.flag = await _roleService.DeleteAsync(ids);
            }
            return Json(result);
        }

        /// <summary>
        /// 搜索页面
        /// </summary>
        /// <param name="filters">查询参数</param>
        /// <returns></returns>
        [IgnoreActionFilterAttribute]
        public async Task<IActionResult> GetListWithPager(RoleFilters filters)
        {
            var result = await _roleService.SearchAsync(filters);
            return Json(result);
        }
    }
}