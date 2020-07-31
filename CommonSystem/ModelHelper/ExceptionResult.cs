using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommonSystem.ModelHelper
{
    public class ExceptionResult:Exception
    {
        public ExceptionResult()
        {

        }

        public ExceptionResult(string message) : base(message)
        {

        }

        public ExceptionResult(string message, Exception ex) : base(message, ex)
        {

        }
    }
}
