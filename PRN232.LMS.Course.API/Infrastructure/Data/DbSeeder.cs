using System;
using System.Collections.Generic;
using System.Linq;
using PRN232.LMS.Course.API.Infrastructure.Data;
using PRN232.LMS.Course.API.Domain.Entities;

namespace PRN232.LMS.Course.API.Infrastructure.Data
{
    public static class DbSeeder
    {
        public static void Seed(CourseDbContext context)
        {
            if (!context.Courses.Any())
            {
                var courses = new List<PRN232.LMS.Course.API.Domain.Entities.Course>();
                var courseNames = new[]
                {
                    "Web API Development", "Software Project Management", "Database Design & Optimization",
                    "Mobile Development with Flutter", "Advanced C# Programming", "Cloud Computing with Azure",
                    "Machine Learning Basics", "Frontend Development with React", "DevOps and Docker",
                    "Cybersecurity Fundamentals", "Microservices Architecture", "Test Automation",
                    "UI/UX Design Principles", "Data Analytics with Python", "IoT Development",
                    "Blockchain Basics", "API Gateway & Load Balancing", "Performance Optimization",
                    "System Design", "Agile Methodologies"
                };

                for (int i = 0; i < courseNames.Length; i++)
                {
                    courses.Add(new PRN232.LMS.Course.API.Domain.Entities.Course
                    {
                        Coursename = courseNames[i],
                        Semesterid = (i % 5) + 1
                    });
                }
                context.Courses.AddRange(courses);
                context.SaveChanges();
            }

            if (!context.Enrollments.Any())
            {
                var random = new Random();
                var enrollments = new List<Enrollment>();
                var enrollmentSet = new HashSet<string>();

                for (int i = 0; i < 500; i++)
                {
                    var studentId = (i % 50) + 1;
                    var courseId = ((i + (i / 50)) % 20) + 1;

                    string key = $"{studentId}_{courseId}";
                    if (enrollmentSet.Contains(key))
                    {
                        courseId = (courseId % 20) + 1;
                        key = $"{studentId}_{courseId}";
                    }

                    if (!enrollmentSet.Contains(key))
                    {
                        enrollmentSet.Add(key);
                        enrollments.Add(new Enrollment
                        {
                            Studentid = studentId,
                            Courseid = courseId,
                            Enrolldate = DateTime.UtcNow.AddDays(-random.Next(1, 180)),
                            Status = random.Next(0, 10) < 8 ? "Active" : "Completed"
                        });
                    }
                }

                context.Enrollments.AddRange(enrollments);
                context.SaveChanges();
            }

            if (!context.Grades.Any())
            {
                var enrollments = context.Enrollments.ToList();
                var random = new Random();
                var grades = new List<Grade>();

                foreach (var enrollment in enrollments)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        grades.Add(new Grade
                        {
                            Enrollmentid = enrollment.Enrollmentid,
                            Subjectid = random.Next(1, 11),
                            Mark = (decimal)(random.Next(70, 100) + random.NextDouble())
                        });
                    }
                }

                context.Grades.AddRange(grades);
                context.SaveChanges();
            }
        }
    }
}