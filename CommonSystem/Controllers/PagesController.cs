using ApplicationLayer.EntityDto.BaseDto;
using CommonSystem.ModelHelper;
using Microsoft.AspNetCore.Mvc;

namespace CommonSystem.Controllers
{
    /// <summary>
    /// 首页
    /// </summary>
    public class PagesController : Controller
    {

        [MeunAttribute(Id = Menu.PageButtonId, ParentId = Menu.PagesId, Name = "按钮", Order = "1")]
        public IActionResult Buttons()
        {
            return View();
        }

        [MeunAttribute(Id = Menu.PageFontId, ParentId = Menu.PagesId, Name = "字体", Order = "2")]
        public IActionResult FontAwesome()
        {
            return View();
        }

        [MeunAttribute(Id = Menu.PageFormId, ParentId = Menu.PagesId, Name = "表单", Order = "3")]
        public IActionResult Form()
        {
            return View();
        }

        [MeunAttribute(Id = Menu.PageFormAdvanceId, ParentId = Menu.PagesId, Name = "高级表单", Order = "4")]
        public IActionResult FormAdvance()
        {
            return View();
        }

        [MeunAttribute(Id = Menu.PageTableId, ParentId = Menu.PagesId, Name = "表格", Order = "5")]
        public IActionResult Tables()
        {
            return View();
        }

        [MeunAttribute(Id = Menu.PageTabId, ParentId = Menu.PagesId, Name = "选项卡", Order = "6")]
        public IActionResult Tabs()
        {
            return View();
        }
    }
}