using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommonSystem.ModelHelper
{
    [AttributeUsage(AttributeTargets.Property,AllowMultiple =false)]
    public class IgnoreAttribute:Attribute
    {

    }
}
