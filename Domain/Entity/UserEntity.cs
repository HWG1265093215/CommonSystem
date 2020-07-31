using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entity
{
    /// <summary>
    /// 用户实体
    /// </summary>
    public class UserEntity:BaseEntity
    {
        /// <summary>
        /// 登录账号
        /// </summary>
        public string LoginName { get; set; }

        /// <summary>
        /// 真实姓名
        /// </summary>
        public string RealName { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 登录密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 是否是超级管理员
        /// </summary>
        public bool IsSuperMan { get; set; }

        /// <summary>
        /// 部门Id
        /// </summary>
        public string DepartmentId { get; set; }

        /// <summary>
        /// 用户拥有的角色
        /// </summary>
        public virtual ICollection<UserRoleEntity> UserRoles { get; set; }

        /// <summary>
        /// 部门
        /// </summary>
        public virtual DepartmentEntity Department { get; set; }
    }
}
