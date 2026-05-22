using PRN232.LMS.Services.BusinessModels;
using PRN232.LMS.Models.Response;

namespace PRN232.LMS.API.Helpers;

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
