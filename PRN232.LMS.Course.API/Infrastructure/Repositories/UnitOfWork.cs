using System;
using System.Threading.Tasks;
using PRN232.LMS.Course.API.Domain.Entities;
using PRN232.LMS.Course.API.Infrastructure.Data;

namespace PRN232.LMS.Course.API.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly CourseDbContext _context;
        public IGenericRepository<PRN232.LMS.Course.API.Domain.Entities.Course> Courses { get; set; }
        public IGenericRepository<Enrollment> Enrollments { get; set; }
        public IGenericRepository<Grade> Grades { get; set; }


        public UnitOfWork(
            CourseDbContext context,
            IGenericRepository<PRN232.LMS.Course.API.Domain.Entities.Course> courses, IGenericRepository<Enrollment> enrollments, IGenericRepository<Grade> grades)
        {
            _context = context;
            Courses = courses;
            Enrollments = enrollments;
            Grades = grades;
        }

        public void Dispose() => _context.Dispose();
        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}