using System.Threading.Tasks;
using ApplicationLayer.EntityDto.BaseDto;
using ApplicationLayer.Filters;
using Domain.Enums;

namespace ApplicationLayer.IService
{
    /// <summary>
    /// 日志契约
    /// </summary>
    public interface ILogService
    {
        /// <summary>
        /// 获取登录日志
        /// </summary>
        /// <param name="filters">过滤器</param>
        /// <returns></returns>
        Task<PagedResult<LoginLogDto>> SearchLoginLogsAsync(LogFilters filters);

        /// <summary>
        /// 获取访问日志
        /// </summary>
        /// <param name="filters">过滤器</param>
        /// <returns></returns>
        Task<PagedResult<VisitDto>> SearchVisitLogsAsync(LogFilters filters);
    }
}
