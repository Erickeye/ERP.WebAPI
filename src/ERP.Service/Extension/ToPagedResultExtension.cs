using System.Linq.Expressions;
using ERP.Library.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace ERP.Library.Extensions
{
    public static class QueryablePagingExtensions
    {
        /// <summary>
        /// 轉換PageList
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query">List查詢字串</param>
        /// <param name="search">查詢Model-取Page及Number</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static async Task<PagedResult<T>> ToPagedResultAsync<T>(
            this IQueryable<T> query,
            SearchModel search)
        {
            if (search == null) throw new ArgumentNullException(nameof(search));

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((search.PageNumber - 1) * search.PageSize)
                .Take(search.PageSize)
                .ToListAsync();

            return new PagedResult<T>
            {
                Items = items,
                PageNumber = search.PageNumber,
                PageSize = search.PageSize,
                TotalCount = totalCount
            };
        }
        /// <summary>
        /// PageList-排序擴充功能
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query">List查詢字串</param>
        /// <param name="search">查詢Model</param>
        /// <param name="allowedColumns"></param>
        /// <returns></returns>
        public static IQueryable<T> ApplySort<T>(
         this IQueryable<T> query,
         SearchModel search,
         IDictionary<string, Expression<Func<T, object>>> allowedColumns)
        {
            if (string.IsNullOrWhiteSpace(search.SortColumn))
                return query;

            var column = allowedColumns
                .FirstOrDefault(x =>string.Equals(x.Key, search.SortColumn, StringComparison.OrdinalIgnoreCase));

            if (column.Value == null)
                return query;

            var isDesc = string.Equals(
                search.SortDirection,
                "desc",
                StringComparison.OrdinalIgnoreCase);

            return isDesc
                ? query.OrderByDescending(column.Value)
                : query.OrderBy(column.Value);
        }
    }

}
