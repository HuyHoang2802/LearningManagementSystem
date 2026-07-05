using System;

namespace PRN232.LMS.Student.API.Domain.Request
{
    public class StudentQueryRequest
    {
        public string? Search { get; set; }
        public string? Sort { get; set; }
        public string? Expand { get; set; }
        public int Page { get; set; }
        public int Size { get; set; }
    }
}
