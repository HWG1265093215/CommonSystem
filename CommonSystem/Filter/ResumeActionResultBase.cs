using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommonSystem.Filter
{
    public class ResumeActionResultBase :ActionResult
    {
        public override void ExecuteResult(ActionContext context)
        {
            base.ExecuteResult(context);
        }
    }
}
