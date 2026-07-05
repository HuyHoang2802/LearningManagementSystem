using System;
using System.Collections.Generic;

namespace PRN232.LMS.Course.API.Domain.Entities;

public partial class Grade
{
    public int Gradeid { get; set; }

    public int Enrollmentid { get; set; }

    public int Subjectid { get; set; }

    public decimal? Mark { get; set; }

    public virtual Enrollment Enrollment { get; set; } = null!;
}
