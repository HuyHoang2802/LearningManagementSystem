using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using PRN232.LMS.Models.Validation;

namespace PRN232.LMS.Models.Request
{
    public class UpdateStudentRequest
    {
        [Required]
        [StringLength(50)]
        [FptuStudentCode]
        public string StudentCode { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }
    }
}
