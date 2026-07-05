using System;
using System.Collections.Generic;

namespace PRN232.LMS.Course.API.Domain.Entities;

public partial class Course
{
    public int Courseid { get; set; }

    public string Coursename { get; set; } = null!;

    public int Semesterid { get; set; }

    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}
