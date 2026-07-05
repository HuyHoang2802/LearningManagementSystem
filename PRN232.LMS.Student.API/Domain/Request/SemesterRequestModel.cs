using System;
using System.ComponentModel.DataAnnotations;

namespace PRN232.LMS.Student.API.Domain.Request
{
    public class SemesterRequestModel
    {
        [Required]
        [MaxLength(100)]
        public string SemesterName { get; set; } = string.Empty;

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}
