using System;
using System.ComponentModel.DataAnnotations;

namespace PRN232.LMS.Student.API.Domain.Request
{
    public class SubjectRequestModel
    {
        [Required]
        [MaxLength(50)]
        public string SubjectCode { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string SubjectName { get; set; } = string.Empty;

        [Range(1, 10)]
        public int Credit { get; set; }
    }
}
