using PRN232.LMS.Models.Entities;
using PRN232.LMS.Repositories.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN232.LMS.Repositories.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly LmsdbContext _context;
        public IGenericRepository<Student> Students { get; set; }

        public UnitOfWork(LmsdbContext context, IGenericRepository<Student> students)
        {
            _context = context;
            Students = students;
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
