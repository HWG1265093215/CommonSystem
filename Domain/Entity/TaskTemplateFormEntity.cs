using System;
using System.Collections.Generic;
using System.Text;
using Domain.Meun;

namespace Domain.Entity
{
    public class TaskTemplateFormEntity:BaseEntity
    {
        /// <summary>
        /// 所属模板Id
        /// </summary>
        public string TemplateId { get; set; }
        /// <summary>
        /// 控件类型
        /// </summary>
        public ControlType ControlType { get; set; }
        /// <summary>
        /// 控件名称
        /// </summary>
        public string ControlName { get; set; }
        /// <summary>
        /// 排序(升序)
        /// </summary>
        public int Order { get; set; }
        /// <summary>
        /// 所属模板
        /// </summary>
        public virtual TaskTemplateEntity Template { get; set; }
    }
}
