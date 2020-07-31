using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entity
{
    public class PathCodeEntity:BaseEntity
    {
        /// <summary>
        /// 路径码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 路径码长度
        /// </summary>
        public int Len { get; set; }
    }
}
