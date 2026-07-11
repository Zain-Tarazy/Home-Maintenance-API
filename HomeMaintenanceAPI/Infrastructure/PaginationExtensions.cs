using HomeMaintenanceAPI.Application.Common;
using Microsoft.EntityFrameworkCore;

namespace HomeMaintenanceAPI.Infrastructure
{
    public static class PaginationExtensions
    {
        public static async Task<PagedResult<T>> ToPagedResultAsync<T>(
            this IQueryable<T> query,
            PaginationParams paginationParams)
        {
            var totalCount = await query.CountAsync();

            if (!paginationParams.IsPaginationEnabled)
            {
                var allItems = await query.ToListAsync();

                return new PagedResult<T>(
                    allItems,
                    pageNumber: 1,
                    pageSize: totalCount,
                    totalCount: totalCount
                );
            }

            var pageNumber = paginationParams.GetPageNumber();
            var pageSize = paginationParams.GetPageSize();

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<T>(
                items,
                pageNumber,
                pageSize,
                totalCount
            );
        }
    }
}
