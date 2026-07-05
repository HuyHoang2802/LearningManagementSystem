using PRN232.LMS.Course.API.Domain.Entities;
using PRN232.LMS.Course.API.Domain.Response;

namespace PRN232.LMS.Course.API.Helpers;

public static class PaginationHelper
{
    public static PagedResponse Create<T>(PagedResultModel<T> result)
    {
        return new PagedResponse
        {
            Page = result.Page,
            PageSize = result.PageSize,
            TotalItems = result.TotalItems,
            TotalPages = result.TotalPages
        };
    }
}