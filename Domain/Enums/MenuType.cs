using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;

namespace Domain.Meun
{
    public enum MenuType : byte
    {
        /// <summary>
        /// 模块
        /// </summary>
        [Description("模块")]
        Module = 1,

        /// <summary>
        /// 菜单
        /// </summary>
        [Description("菜单")]
        Menu = 2,

        /// <summary>
        /// 操作
        /// </summary>
        [Description("操作")]
        Action = 3
    }
}
