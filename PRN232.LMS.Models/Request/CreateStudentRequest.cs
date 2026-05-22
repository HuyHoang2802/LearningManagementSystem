using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN232.LMS.Models.Request
{
    public class CreateStudentRequest
    {
        public string FullName { get; set; }

        public string Email { get; set; }

        public DateTime DateOfBirth { get; set; }
    }
}
