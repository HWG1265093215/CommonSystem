﻿using System.Collections.Generic;
using System.Threading.Tasks;
using ApplicationLayer.EntityDto.BaseDto;

namespace ApplicationLayer.IService
{
    /// <summary>
    /// 数据库初始化契约
    /// </summary>
    public interface IDatabaseInitService
    {
        /// <summary>
        /// 初始化数据库数据
        /// </summary>
        Task<bool> InitAsync(List<MenuDto> menues);

        /// <summary>
        /// 初始化路径码
        /// </summary>
        Task<bool> InitPathCodeAsync();

        /// <summary>
        /// 初始化省市区数据
        /// </summary>
        /// <returns></returns>
        Task<bool> InitAreas();
    }
}
