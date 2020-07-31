using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entity
{
    public class UserRoleEntity:BaseEntity
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 角色ID
        /// </summary>
        public string RoleId { get; set; }

        /// <summary>
        /// 所属用户
        /// </summary>
        public virtual UserEntity User { get; set; }

        /// <summary>
        /// 角色
        /// </summary>
        public virtual RoleEntity Role { get; set; }
    }
}
