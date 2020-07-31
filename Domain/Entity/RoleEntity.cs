using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entity
{
    public class RoleEntity:BaseEntity
    {
        /// <summary>
        /// 角色名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 用户角色关系
        /// </summary>
        public virtual IList<UserRoleEntity> UserRoles { get; set; }

        /// <summary>
        /// 角色菜单关系
        /// </summary>
        public virtual IList<RoleMenuEntity> RoleMenus { get; set; }
    }
}
