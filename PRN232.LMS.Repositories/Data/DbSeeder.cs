using PRN232.LMS.Models.Entities;

namespace PRN232.LMS.Repositories.Data
{
    public static class DbSeeder
    {
        public static void Seed(LmsdbContext context)
        {
            // SUBJECTS - 10 subjects
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

            // STUDENTS - 50 students
            if (!context.Students.Any())
            {
                var students = new List<Student>();
                for (int i = 1; i <= 50; i++)
                {
                    students.Add(new Student
                    {
                        Fullname = $"Student {i:D2}",
                        Email = $"student{i}@fpt.edu.vn",
                        Dateofbirth = new DateOnly(2002 + (i % 4), (i % 12) + 1, (i % 28) + 1)
                    });
                }
                context.Students.AddRange(students);
                context.SaveChanges();
            }

            // SEMESTERS - 5 semesters
            if (!context.Semesters.Any())
            {
                context.Semesters.AddRange(
                    new Semester
                    {
                        Semestername = "Spring 2025",
                        Startdate = new DateOnly(2025, 1, 15),
                        Enddate = new DateOnly(2025, 5, 31)
                    },
                    new Semester
                    {
                        Semestername = "Summer 2025",
                        Startdate = new DateOnly(2025, 6, 1),
                        Enddate = new DateOnly(2025, 8, 31)
                    },
                    new Semester
                    {
                        Semestername = "Fall 2025",
                        Startdate = new DateOnly(2025, 9, 1),
                        Enddate = new DateOnly(2025, 12, 31)
                    },
                    new Semester
                    {
                        Semestername = "Spring 2026",
                        Startdate = new DateOnly(2026, 1, 15),
                        Enddate = new DateOnly(2026, 5, 31)
                    },
                    new Semester
                    {
                        Semestername = "Summer 2026",
                        Startdate = new DateOnly(2026, 6, 1),
                        Enddate = new DateOnly(2026, 8, 31)
                    }
                );
                context.SaveChanges();
            }

            // COURSES - 20 courses
            if (!context.Courses.Any())
            {
                var semesters = context.Semesters.ToList();
                var courses = new List<Course>();
                var courseNames = new[]
                {
                    "Web API Development",
                    "Software Project Management",
                    "Database Design & Optimization",
                    "Mobile Development with Flutter",
                    "Advanced C# Programming",
                    "Cloud Computing with Azure",
                    "Machine Learning Basics",
                    "Frontend Development with React",
                    "DevOps and Docker",
                    "Cybersecurity Fundamentals",
                    "Microservices Architecture",
                    "Test Automation",
                    "UI/UX Design Principles",
                    "Data Analytics with Python",
                    "IoT Development",
                    "Blockchain Basics",
                    "API Gateway & Load Balancing",
                    "Performance Optimization",
                    "System Design",
                    "Agile Methodologies"
                };

                for (int i = 0; i < courseNames.Length; i++)
                {
                    courses.Add(new Course
                    {
                        Coursename = courseNames[i],
                        Semesterid = semesters[i % semesters.Count].Semesterid
                    });
                }
                context.Courses.AddRange(courses);
                context.SaveChanges();
            }

            // ENROLLMENTS - 500 enrollments
            if (!context.Enrollments.Any())
            {
                var students = context.Students.ToList();
                var courses = context.Courses.ToList();
                var random = new Random();
                var enrollments = new List<Enrollment>();
                var enrollmentSet = new HashSet<string>();

                // Create 500 enrollments with varied patterns
                for (int i = 0; i < 500; i++)
                {
                    var studentIdx = i % students.Count;
                    var courseIdx = (i + (i / students.Count)) % courses.Count;
                    
                    // Avoid duplicate enrollment (student + course combination)
                    string key = $"{students[studentIdx].Studentid}_{courses[courseIdx].Courseid}";
                    if (enrollmentSet.Contains(key))
                    {
                        courseIdx = (courseIdx + 1) % courses.Count;
                        key = $"{students[studentIdx].Studentid}_{courses[courseIdx].Courseid}";
                    }
                    
                    if (!enrollmentSet.Contains(key))
                    {
                        enrollmentSet.Add(key);
                        enrollments.Add(new Enrollment
                        {
                            Studentid = students[studentIdx].Studentid,
                            Courseid = courses[courseIdx].Courseid,
                            Enrolldate = DateTime.UtcNow.AddDays(-random.Next(1, 180)),
                            Status = random.Next(0, 10) < 8 ? "Active" : "Completed"
                        });
                    }
                }

                context.Enrollments.AddRange(enrollments);
                context.SaveChanges();
            }

            // GRADES - Multiple grades per enrollment
            if (!context.Grades.Any())
            {
                var enrollments = context.Enrollments.ToList();
                var subjects = context.Subjects.ToList();
                var random = new Random();

                var grades = new List<Grade>();
                foreach (var enrollment in enrollments)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        if (subjects.Count > 0)
                        {
                            grades.Add(new Grade
                            {
                                Enrollmentid = enrollment.Enrollmentid,
                                Subjectid = subjects[random.Next(subjects.Count)].Subjectid,
                                Mark = (decimal)(random.Next(70, 100) + random.NextDouble())
                            });
                        }
                    }
                }

                context.Grades.AddRange(grades);
                context.SaveChanges();
            }
        }
    }
}
