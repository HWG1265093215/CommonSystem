﻿using ApplicationLayer.EntityDto.BaseDto;
using ApplicationLayer.Filters;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.IService
{
    public interface IModelTempService
    {
        Task<PagedResult<ModelTempDto>> SearchAsync(BaseFilter baseFilter);

        Task<Boolean> Add(ModelTempDto tempDto);

        Task<ModelTempDto> Find(string id);

        Task<bool> DeleteAsync(List<string> ids);
    }
}
