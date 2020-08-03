using System;
using System.Collections.Generic;
using System.Text;
using Domain.Meun;

namespace Domain.Entity
{
    public partial class TaskTemplateEntity:BaseEntity
    {
        /// <summary>
        /// 任务流模板名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 设计步骤
        /// </summary>
        public TaskTemplateStep Step { get; set; }
        /// <summary>
        /// 表单集合
        /// </summary>
        public virtual IList<TaskTemplateFormEntity> Forms { get; set; }
        /// <summary>
        /// 步骤集合
        /// </summary>
        public virtual IList<TaskTemplateStepEntity> Steps { get; set; }
    }
}
