using Microsoft.EntityFrameworkCore;
using ERP.Library.ViewModels;

namespace ERP.Library.Extensions
{
    public static class QueryablePagingExtensions
    {
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
    }

}
