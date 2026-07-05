using System;
using System.Collections.Generic;

namespace PRN232.LMS.Student.API.Domain.Entities;

public partial class Subject
{
    public int Subjectid { get; set; }

    public string Subjectcode { get; set; } = null!;

    public string Subjectname { get; set; } = null!;

    public int Credit { get; set; }
}
