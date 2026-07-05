using System;
using System.ComponentModel.DataAnnotations;
using PRN232.LMS.Student.API.Domain.Validation;

namespace PRN232.LMS.Student.API.Domain.Request
{
    public class UpdateStudentRequest
    {
        [Required]
        [StringLength(50)]
        [FptuStudentCode]
        public string StudentCode { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public DateTime DateOfBirth { get; set; }
    }
}
