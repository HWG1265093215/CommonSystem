using System;

namespace ApplicationLayer.EntityDto.BaseDto
{
    /// <summary>
    /// 网站访问量数据
    /// </summary>
    public class SiteViewDto
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
