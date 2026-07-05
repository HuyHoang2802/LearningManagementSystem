using System;
using System.Threading.Tasks;
using PRN232.LMS.Student.API.Domain.Entities;
using PRN232.LMS.Student.API.Infrastructure.Data;

namespace PRN232.LMS.Student.API.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly StudentDbContext _context;
        public IGenericRepository<PRN232.LMS.Student.API.Domain.Entities.Student> Students { get; set; }
        public IGenericRepository<Semester> Semesters { get; set; }
        public IGenericRepository<Subject> Subjects { get; set; }


        public UnitOfWork(
            StudentDbContext context,
            IGenericRepository<PRN232.LMS.Student.API.Domain.Entities.Student> students, IGenericRepository<Semester> semesters, IGenericRepository<Subject> subjects)
        {
            _context = context;
            Students = students;
            Semesters = semesters;
            Subjects = subjects;
        }

        public void Dispose() => _context.Dispose();
        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}