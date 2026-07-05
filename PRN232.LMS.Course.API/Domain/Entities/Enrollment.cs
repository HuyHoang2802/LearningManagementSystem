using System;
using System.Collections.Generic;

namespace PRN232.LMS.Course.API.Domain.Entities;

public partial class Enrollment
{
    public int Enrollmentid { get; set; }

    public int Studentid { get; set; }

    public int Courseid { get; set; }

    public DateTime? Enrolldate { get; set; }

    public string? Status { get; set; }

    public virtual Course Course { get; set; } = null!;

    public virtual ICollection<Grade> Grades { get; set; } = new List<Grade>();
}
