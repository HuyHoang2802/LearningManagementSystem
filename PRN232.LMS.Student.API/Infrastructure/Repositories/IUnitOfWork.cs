using System;
using System.Threading.Tasks;
using PRN232.LMS.Student.API.Domain.Entities;

namespace PRN232.LMS.Student.API.Infrastructure.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        public IGenericRepository<PRN232.LMS.Student.API.Domain.Entities.Student> Students { get; set; }
        public IGenericRepository<Semester> Semesters { get; set; }
        public IGenericRepository<Subject> Subjects { get; set; }

        Task<int> SaveChangesAsync();
    }
}