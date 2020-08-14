using Autofac;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace CommonSystem.MiddleWare
{
    public class DefaultModule:Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var ControllerBaseType = typeof(Controller);
            //IsAssignableFrom 是否间接或直接实现了Controller 类型 
            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly()).Where(n => ControllerBaseType.IsAssignableFrom(n) && n != ControllerBaseType).PropertiesAutowired();
        }
    }
}
