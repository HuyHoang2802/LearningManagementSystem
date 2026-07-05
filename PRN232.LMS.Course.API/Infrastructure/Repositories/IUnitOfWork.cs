using System;
using System.Threading.Tasks;
using PRN232.LMS.Course.API.Domain.Entities;

namespace PRN232.LMS.Course.API.Infrastructure.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        public IGenericRepository<PRN232.LMS.Course.API.Domain.Entities.Course> Courses { get; set; }
        public IGenericRepository<Enrollment> Enrollments { get; set; }
        public IGenericRepository<Grade> Grades { get; set; }

        Task<int> SaveChangesAsync();
    }
}