using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entity
{
    public class SiteViewEntity:BaseEntity
    {
        /// <summary>
        /// 日期
        /// </summary>
        public DateTime Day { get; set; }

        /// <summary>
        /// 访问量
        /// </summary>
        public int Number { get; set; }
    }
}
