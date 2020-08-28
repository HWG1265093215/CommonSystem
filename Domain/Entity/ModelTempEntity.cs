using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entity
{
    public class ModelTempEntity : BaseEntity
    {
        public string TempName{ get; set; }

        public string ContentPath { get; set; }

        public string TempTable { get; set; }
    }
}
