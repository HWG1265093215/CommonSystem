using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationLayer.ExtendHelper
{
    [AttributeUsage(AttributeTargets.Property,AllowMultiple =false)]
    public class IgnoreAttribute:Attribute
    {

    }
}
