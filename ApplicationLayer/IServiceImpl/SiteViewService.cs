using ApplicationLayer.EntityDto.BaseDto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationLayer.IService;
using Domain;
using Domain.Entity;

namespace ApplicationLayer.IServiceImpl
{
    public class SiteViewService : ISiteViewService
    {
        private readonly DBContext _context;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="dbContextScopeFactory"></param>
        public SiteViewService(DBContext context)
        {
            _context = context;
        }


        /// <summary>
        /// 添加或更新网站访问量
        /// </summary>
        /// <param name="day">日期</param>
        public async Task<bool> AddOrUpdate()
        {
            var day = DateTime.Now;
            var date = day.Date;
            var endDate = date.AddDays(1);
            var number = await _context.PageViews.CountAsync(x => x.CreateTime > date && x.CreateTime < endDate);
            if (await _context.SiteViews.AnyAsync(x => x.Day == date))
            {
                var view = await _context.SiteViews.FirstOrDefaultAsync(x => x.Day == date);
                if (view != null)
                {
                    view.Number = number;
                }
            }
            else
            {
                var view = new SiteViewEntity
                {
                    Day = date,
                    Number = number
                };
                view.InitEntity();
                _context.SiteViews.Add(view);
            }
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// 获取指定时间段内的访问量统计数据
        /// </summary>
        /// <param name="from">开始时间</param>
        /// <param name="to">结束时间</param>
        /// <returns></returns>
        public async Task<IList<SiteViewDto>> GetSiteViews(DateTime from, DateTime to)
        {
            var start = from.Date.AddDays(-1);
            var end = to.Date.AddDays(1);
            return
                await
                    _context.SiteViews.Where(x => x.Day > start && x.Day < end)
                        .Select(x => new SiteViewDto { Day = x.Day, Number = x.Number })
                        .ToListAsync();
        }
    }
}
