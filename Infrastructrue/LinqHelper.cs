using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Infrastructrue
{
    public static class LinqHelper
    {
        /// <summary>
        /// LINQ扩展方法
        /// </summary>
         
            /// <summary>
            /// 与连接
            /// </summary>
            /// <typeparam name="T">类型</typeparam>
            /// <param name="left">左条件</param>
            /// <param name="right">右条件</param>
            /// <returns>新表达式</returns>
            public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
            {
                return CombineLambdas(left, right, ExpressionType.AndAlso);
            }

            /// <summary>
            /// 或连接
            /// </summary>
            /// <typeparam name="T">类型</typeparam>
            /// <param name="left">左条件</param>
            /// <param name="right">右条件</param>
            /// <returns>新表达式</returns>
            public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
            {
                return CombineLambdas(left, right, ExpressionType.OrElse);
            }

            private static Expression<Func<T, bool>> CombineLambdas<T>(this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right, ExpressionType expressionType)
            {
                if (IsExpressionBodyConstant(left))
                {
                    return right;
                }

                var visitor = new SubstituteParameterVisitor
                {
                    Sub =
                {
                    [right.Parameters[0]] = left.Parameters[0]
                }
                };

                Expression body = Expression.MakeBinary(expressionType, left.Body, visitor.Visit(right.Body));
                return Expression.Lambda<Func<T, bool>>(body, left.Parameters[0]);
            }

            private static bool IsExpressionBodyConstant<T>(Expression<Func<T, bool>> left)
            {
                return left.Body.NodeType == ExpressionType.Constant;
            }

            internal class SubstituteParameterVisitor : ExpressionVisitor
            {
                public Dictionary<Expression, Expression> Sub = new Dictionary<Expression, Expression>();

                protected override Expression VisitParameter(ParameterExpression node)
                {
                    return Sub.TryGetValue(node, out var newValue) ? newValue : node;
                }
            }

            /// <summary>
            /// WhereIf[在condition为true的情况下应用Where表达式]
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="source"></param>
            /// <param name="condition"></param>
            /// <param name="expression"></param>
            /// <returns></returns>
            public static IQueryable<T> WhereIf<T>(this IQueryable<T> source, bool condition, Expression<Func<T, bool>> expression)
            {
                return condition ? source.Where(expression) : source;
            }

            /// <summary>
            /// 分页
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="source"></param>
            /// <param name="pageIndex">当前页</param>
            /// <param name="pageSize">页大小</param>
            /// <returns></returns>
            public static PagedResult<T> Paging<T>(this IQueryable<T> source, int pageIndex, int pageSize)
            {
                if (pageIndex <= 0)
                    throw new ArgumentException("Index of current page can not less than 0 !", "pageIndex");
                if (pageSize <= 1)
                    throw new ArgumentException("Size of page can not less than 1 !", "pageSize");

                var pagedQuery = source
                    .Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize);

                return new PagedResult<T>
                {
                    rows = pagedQuery.ToList(),
                    records = source.Count(),

                    page = pageIndex,
                    pagesize = pageSize
                };
            }

            /// <summary>
            /// 分页
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="source"></param>
            /// <param name="pageIndex">当前页</param>
            /// <param name="pageSize">页大小</param>
            /// <returns></returns>
            public static async Task<PagedResult<T>> PagingAsync<T>(this IQueryable<T> source, int pageIndex, int pageSize)
            {
                if (pageIndex <= 0)
                    throw new ArgumentException("Index of current page can not less than 0 !", "pageIndex");
                if (pageSize <= 1)
                    throw new ArgumentException("Size of page can not less than 1 !", "pageSize");

                var pagedQuery = source
                    .Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize);

                return new PagedResult<T>
                {
                    rows = await pagedQuery.ToListAsync(),
                    records = await source.CountAsync(),

                    page = pageIndex,
                    pagesize = pageSize
                };
            }
    }
}
