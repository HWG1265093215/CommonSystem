using ApplicationLayer.EntityDto.BaseDto;
using ApplicationLayer.Filters;
using ApplicationLayer.IService;
using Domain;
using Domain.Enums;
using Infrastructrue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.IServiceImpl
{
    public class ModelTempServiceImpl : IModelTempService
    {
        public DBContext _context { get; set; }
         
        public async Task<PagedResult<ModelTempDto>> SearchAsync(BaseFilter baseFilter)
        {
            if (baseFilter == null) return new PagedResult<ModelTempDto>();
            var query = _context.ModelTemp.AsQueryable();
            if(baseFilter.keywords.IsNotBlank())
            {
               query=query.Where(n => n.TempTable.Contains(baseFilter.keywords) || n.TempName.Contains(baseFilter.keywords));
            }
            return await query.OrderByDescending(item => item.CreateTime)
                   .Select(x => new ModelTempDto
                   {
                       Id = x.Id,
                       TempName = x.TempName,
                       ContentPath = x.ContentPath,
                       TempTable = x.TempTable
                   }).PagingAsync(baseFilter.page,baseFilter.rows);
        }
    }
}
