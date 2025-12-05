using MovieAPI.DTOs;

namespace MovieAPI.Utils;

public static class IQueriableExtentions
{
    public static IQueryable<T> Paginate<T>(this IQueryable<T> query, PaginationDTO pagination)
    {
        return query.Skip((pagination.page - 1) * pagination.pageSize).Take(pagination.pageSize);
    }
}