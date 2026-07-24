
using Microsoft.EntityFrameworkCore;
using TaskManagement.Dtos.Common;

namespace TaskManagement.ExtensionMethods;

public static class IQueryableExtensions
{
    public static async Task<PaginatedList<T>> ToPaginatedListAsync<T>(this IQueryable<T> source, PaginationQueryParameters parameters, CancellationToken ct = default)
    {
        int limit = parameters.Limit;
        int index = parameters.PageIndex;
        int count = await source.CountAsync(ct);
        int totalPages = (int)Math.Ceiling((double)count / limit);

        if (totalPages <= 0)
            return new PaginatedList<T>([], 0, 0, 0);

        if (index > totalPages)
            index = totalPages;

        var pageItems = await source.Skip((index - 1) * limit)
                                .Take(limit)
                                .ToListAsync(ct);

        return new PaginatedList<T>(
        pageItems: pageItems,
        totalPages: totalPages,
        totalCount: count,
        pageIndex: index);
    }
}