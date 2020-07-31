using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entity
{
    public class RoleMenuEntity:BaseEntity
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        public string RoleId { get; set; }

        /// <summary>
        /// 菜单ID
        /// </summary>
        public string MenuId { get; set; }

        /// <summary>
        /// 角色
        /// </summary>
        public virtual RoleEntity Role { get; set; }

        /// <summary>
        /// 菜单
        /// </summary>
        public virtual MenuEntity Menu { get; set; }
    }
}
