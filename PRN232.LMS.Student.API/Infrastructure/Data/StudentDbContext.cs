using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Student.API.Domain.Entities;

namespace PRN232.LMS.Student.API.Infrastructure.Data
{
    public class StudentDbContext : DbContext
    {
        public StudentDbContext(DbContextOptions<StudentDbContext> options) : base(options)
        {
        }

        public virtual DbSet<PRN232.LMS.Student.API.Domain.Entities.Student> Students { get; set; }
        public virtual DbSet<Semester> Semesters { get; set; }
        public virtual DbSet<Subject> Subjects { get; set; }

    }
}