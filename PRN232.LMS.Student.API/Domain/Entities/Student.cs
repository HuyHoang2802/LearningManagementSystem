using System;
using System.Collections.Generic;

namespace PRN232.LMS.Student.API.Domain.Entities;

public partial class Student
{
    public int Studentid { get; set; }

    public string StudentCode { get; set; } = null!;

    public string Fullname { get; set; } = null!;

    public string Email { get; set; } = null!;

    public DateOnly? Dateofbirth { get; set; }
}
