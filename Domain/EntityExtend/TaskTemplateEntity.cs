using System;
using System.Collections.Generic;
using System.Text;
using Domain.Meun;

namespace Domain.Entity
{
    public partial class TaskTemplateEntity
    {
        /// <summary>
        /// 设置完成步骤
        /// </summary>
        /// <param name="step"></param>
        public void SetStep(TaskTemplateStep step)
        {
            if (step > Step)
                Step = step;
        }
    }
}
