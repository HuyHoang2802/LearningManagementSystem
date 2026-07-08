using PRN232.LMS.Models.Entities;

namespace PRN232.LMS.Repositories.IRepositories
{
    public interface IUnitOfWork : IDisposable
    {
        Task<int> SaveChangesAsync();
        IGenericRepository<Student> Students { get; }
        IGenericRepository<Course> Courses { get; }
        IGenericRepository<Enrollment> Enrollments { get; }
        IGenericRepository<Semester> Semesters { get; }
        IGenericRepository<Subject> Subjects { get; }
    }
}
