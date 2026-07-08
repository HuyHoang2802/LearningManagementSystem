using PRN232.LMS.Models.Entities;
using PRN232.LMS.Repositories.IRepositories;

namespace PRN232.LMS.Repositories.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly LmsdbContext _context;
        public IGenericRepository<Student> Students { get; set; }
        public IGenericRepository<Course> Courses { get; set; }
        public IGenericRepository<Enrollment> Enrollments { get; set; }
        public IGenericRepository<Semester> Semesters { get; set; }
        public IGenericRepository<Subject> Subjects { get; set; }

        public UnitOfWork(
            LmsdbContext context,
            IGenericRepository<Student> students,
            IGenericRepository<Course> courses,
            IGenericRepository<Enrollment> enrollments,
            IGenericRepository<Semester> semesters,
            IGenericRepository<Subject> subjects)
        {
            _context = context;
            Students = students;
            Courses = courses;
            Enrollments = enrollments;
            Semesters = semesters;
            Subjects = subjects;
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
