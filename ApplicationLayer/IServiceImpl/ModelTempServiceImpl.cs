using ApplicationLayer.EntityDto.BaseDto;
using ApplicationLayer.ExtendHelper;
using ApplicationLayer.Filters;
using ApplicationLayer.IService;
using AutoMapper;
using Domain;
using Domain.Entity;
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

        public IMapper _mapper { get; set; }

        public async Task<bool> Add(ModelTempDto model)
        {
            var entity =  _context.ModelTemp.Find(model.Id) ;

            string sql = string.Empty;
            try
            {
                if (entity.Id.IsNotBlank())
                {
                    
                    entity = _mapper.MapperSource<ModelTempDto, ModelTempEntity>(model,entity);
                    _context.Entry(entity).State = Microsoft.EntityFrameworkCore.EntityState.Modified;

                    sql = GeneralSqlHelper<ModelTempEntity>.GeneralSQL(SQLEnum.Update);
                    entity.LastUpdateTime = DateTime.Now;

                }
                else
                {
                    entity.InitEntity();
                    _context.ModelTemp.Add(entity);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            
           await _context.SaveChangesAsync();
            return true;
        }

        public async Task<PagedResult<ModelTempDto>> SearchAsync(BaseFilter baseFilter)
        {
            if (baseFilter == null) return new PagedResult<ModelTempDto>();
            var query = _context.ModelTemp.AsQueryable().Where(n=>n.IsDeleted==false);
            if(baseFilter.keywords.IsNotBlank())
            {
               query=query.Where(n => n.TempTable.Contains(baseFilter.keywords) || n.TempName.Contains(baseFilter.keywords));
            }
            return await query.OrderByDescending(item => item.CreateTime)
                   .Select(x =>_mapper.Map<ModelTempEntity,ModelTempDto>(x)).PagingAsync(baseFilter.page,baseFilter.rows);
        }

        public async Task<ModelTempDto> Find(string id)
        {
            var result = await _context.ModelTemp.FindAsync(id);

            var temp = _mapper.Map<ModelTempEntity, ModelTempDto>(result);

            return temp;
        }

        public async Task<bool> DeleteAsync(List<string> list)
        {
            var ModelUser = _context.ModelTemp.Where(n => list.Contains(n.Id));
            ModelUser.NewForEach(item=>item.IsDeleted=true);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
