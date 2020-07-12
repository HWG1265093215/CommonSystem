using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infrastructrue
{
    public class PagedList<T>
    {
        /// <summary>
        /// 数据集
        /// </summary>
        public List<T> Data { get; }

        /// <summary>
        /// 当前页
        /// </summary>
        public int CurrentPage { get; }

        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalPages { get; }

        /// <summary>
        /// 页大小
        /// </summary>
        public int PageSize { get; }

        /// <summary>
        /// 总条数
        /// </summary>
        public int TotalCount { get; }

        /// <summary>
        /// 当前页数据条数
        /// </summary>
        public int CurrentCount => Data.Count;

        /// <summary>
        /// 是否有前一页
        /// </summary>
        public bool HasPrev => CurrentPage > 1;

        /// <summary>
        /// 是否有后一页
        /// </summary>
        public bool HasNext => CurrentPage < TotalPages;
        /// <summary>
        ///  分页数据
        /// </summary>
        /// <param name="items">数据集</param>
        /// <param name="page">当前页</param>
        /// <param name="size">页数</param>
        /// <param name="count">总条数</param>
        public PagedList(List<T> items, int size, int count, int page)
        {
            TotalCount = count;
            PageSize = size;
            CurrentPage = page;
            TotalPages = (int)Math.Ceiling(count * 1.0 / size);
            Data = items;
        }
    }

    public static class IQueryableExtend
    {
        public static PagedList<T> ToPageList<T>(this IOrderedQueryable<T> query,int size, int page = 20)
        {
            //获取总页数
            double totalQuery = query.Count();
            if(page*size>totalQuery)
            {
                page = (int)Math.Ceiling(totalQuery/(size*10));
            }

            if(page<=0)
            {
                page = 1;
            }
            var list = query.Skip(size * (page - 1)).Take(size).ToList();
            return new PagedList<T>((List<T>)query,page,size,(int)totalQuery);
        }
    }

}
