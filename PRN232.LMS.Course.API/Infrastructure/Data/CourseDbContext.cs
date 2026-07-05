using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Course.API.Domain.Entities;

namespace PRN232.LMS.Course.API.Infrastructure.Data
{
    public class CourseDbContext : DbContext
    {
        public CourseDbContext(DbContextOptions<CourseDbContext> options) : base(options)
        {
        }

        public virtual DbSet<PRN232.LMS.Course.API.Domain.Entities.Course> Courses { get; set; }
        public virtual DbSet<Enrollment> Enrollments { get; set; }
        public virtual DbSet<Grade> Grades { get; set; }

    }
}