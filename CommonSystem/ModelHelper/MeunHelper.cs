using ApplicationLayer.EntityDto.BaseDto;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using Infrastructrue;
using System.Collections.Immutable;

namespace CommonSystem.ModelHelper
{
    public class MeunHelper
    {
        public static List<MenuDto> GetMeunes()
        {
            var meuns = new List<MenuDto>()
            {
                //系统管理模块
                new MenuDto{Id=Menu.System.Id,Name=Menu.System.Name,Icon="fa fa-gear"},
                 new MenuDto{Id = Menu.Logs.Id,Name = Menu.Logs.Name,Icon = "fa fa-bars"},
                new MenuDto{Id = Menu.Pages.Id,Name = Menu.Pages.Name,Icon = "fa fa-file-o"}
            };
            //获取当前程序集下所有控制器
            var controller = typeof(Startup).Assembly.GetTypes().Where(n => n.Name.EndsWith("Controller", StringComparison.OrdinalIgnoreCase));

            var temp = from type in typeof(Startup).Assembly.GetTypes() where type.Name.EndsWith("Controller", StringComparison.OrdinalIgnoreCase) select type;

            foreach (var item in controller)
            {
                //获取控制器名称
                var ControllerName = item.Name.Replace("Controller",string.Empty);
                //获取控制器下所有成员  并且使用了属性
                var members = item.GetMembers().Where(m => m.IsDefined(typeof(MeunAttribute)));

                foreach (var action in members)
                {
                    var attr = action.GetCustomAttributes<MeunAttribute>().FirstOrDefault();

                    var actionName = action.Name;

                    var newMeun = new MenuDto
                    {
                        Id = attr.Id,
                        ParentId = attr.ParentId,
                        Name = attr.Name,
                        Order = attr.Order.ToInt(),
                        Url=$"/{ControllerName}/{actionName}"
                    };

                    if(meuns.Any(x=>x.Id== newMeun.Id))
                    {
                        throw new Exception($"已经存在相同的Id={newMeun.Id},Name={newMeun.Name}");
                    }
                    meuns.Add(newMeun);
                }
            }
            return meuns;
        }
    }


}
