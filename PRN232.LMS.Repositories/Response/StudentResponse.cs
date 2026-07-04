using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN232.LMS.Models.Response
{
    public class StudentResponse
    {
        public int? StudentId { get; set; }

        public string? FullName { get; set; }

        public string Email { get; set; }

        public DateOnly? DateOfBirth { get; set; }

        public List<StudentEnrollmentResponse>? Enrollments { get; set; }
    }
}
