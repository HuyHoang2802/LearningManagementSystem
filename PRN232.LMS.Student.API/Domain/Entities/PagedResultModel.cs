using System;
using System.Collections.Generic;

namespace PRN232.LMS.Student.API.Domain.Entities
{
    public class PagedResultModel<T>
    {
        public IReadOnlyList<T> Items { get; set; } = Array.Empty<T>();
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
    }
}