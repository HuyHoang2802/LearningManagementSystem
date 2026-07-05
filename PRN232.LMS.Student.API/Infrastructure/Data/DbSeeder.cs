using System;
using System.Collections.Generic;
using System.Linq;
using PRN232.LMS.Student.API.Infrastructure.Data;
using PRN232.LMS.Student.API.Domain.Entities;

namespace PRN232.LMS.Student.API.Infrastructure.Data
{
    public static class DbSeeder
    {
        public static void Seed(StudentDbContext context)
        {
            if (!context.Subjects.Any())
            {
                context.Subjects.AddRange(
                    new Subject { Subjectcode = "PRN232", Subjectname = "Web API", Credit = 3 },
                    new Subject { Subjectcode = "SWP391", Subjectname = "Software Project", Credit = 4 },
                    new Subject { Subjectcode = "DBI202", Subjectname = "Database Systems", Credit = 3 },
                    new Subject { Subjectcode = "MAD101", Subjectname = "Mobile Development", Credit = 3 },
                    new Subject { Subjectcode = "OSG202", Subjectname = "Operating Systems", Credit = 2 },
                    new Subject { Subjectcode = "CSD201", Subjectname = "Data Structures", Credit = 3 },
                    new Subject { Subjectcode = "PRJ301", Subjectname = "Java Web", Credit = 3 },
                    new Subject { Subjectcode = "SWR302", Subjectname = "Software Requirements", Credit = 2 },
                    new Subject { Subjectcode = "MOB103", Subjectname = "Flutter Basic", Credit = 3 },
                    new Subject { Subjectcode = "NET181", Subjectname = "Network Fundamentals", Credit = 2 }
                );
                context.SaveChanges();
            }

            if (!context.Students.Any())
            {
                var students = new List<PRN232.LMS.Student.API.Domain.Entities.Student>();
                for (int i = 1; i <= 50; i++)
                {
                    students.Add(new PRN232.LMS.Student.API.Domain.Entities.Student
                    {
                        StudentCode = $"SE{180000 + i}",
                        Fullname = $"Student {i:D2}",
                        Email = $"student{i}@fpt.edu.vn",
                        Dateofbirth = new DateOnly(2002 + (i % 4), (i % 12) + 1, (i % 28) + 1)
                    });
                }
                context.Students.AddRange(students);
                context.SaveChanges();
            }

            if (!context.Semesters.Any())
            {
                context.Semesters.AddRange(
                    new Semester { Semestername = "Spring 2025", Startdate = new DateOnly(2025, 1, 15), Enddate = new DateOnly(2025, 5, 31) },
                    new Semester { Semestername = "Summer 2025", Startdate = new DateOnly(2025, 6, 1), Enddate = new DateOnly(2025, 8, 31) },
                    new Semester { Semestername = "Fall 2025", Startdate = new DateOnly(2025, 9, 1), Enddate = new DateOnly(2025, 12, 31) },
                    new Semester { Semestername = "Spring 2026", Startdate = new DateOnly(2026, 1, 15), Enddate = new DateOnly(2026, 5, 31) },
                    new Semester { Semestername = "Summer 2026", Startdate = new DateOnly(2026, 6, 1), Enddate = new DateOnly(2026, 8, 31) }
                );
                context.SaveChanges();
            }
        }
    }
}