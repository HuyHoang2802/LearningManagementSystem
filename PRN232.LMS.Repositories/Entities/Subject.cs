using System;
using System.Collections.Generic;

namespace PRN232.LMS.Models.Entities;

public partial class Subject
{
    public int Subjectid { get; set; }

    public string Subjectcode { get; set; } = null!;

    public string Subjectname { get; set; } = null!;

    public int Credit { get; set; }

    public virtual ICollection<Grade> Grades { get; set; } = new List<Grade>();

    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();
}
