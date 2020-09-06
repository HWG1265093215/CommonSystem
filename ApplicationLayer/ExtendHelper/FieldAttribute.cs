using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationLayer.ExtendHelper
{
    [AttributeUsage(AttributeTargets.All,Inherited =false)]
    public class FieldAttribute:Attribute
    {
        private string fieldName { get; set; }

        public FieldAttribute(string attributeName)
        {
            fieldName = attributeName;
        }

        public string FieldName => fieldName;
    }
}
