using System;
using System.Collections.Generic;

namespace PRN232.LMS.Student.API.Domain.Entities;

public partial class Semester
{
    public int Semesterid { get; set; }

    public string Semestername { get; set; } = null!;

    public DateOnly Startdate { get; set; }

    public DateOnly Enddate { get; set; }
}
