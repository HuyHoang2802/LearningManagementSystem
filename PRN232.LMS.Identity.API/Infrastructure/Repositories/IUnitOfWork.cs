using System;
using System.Threading.Tasks;
using PRN232.LMS.Identity.API.Domain.Entities;

namespace PRN232.LMS.Identity.API.Infrastructure.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        public IGenericRepository<User> Users { get; set; }
        public IGenericRepository<RefreshToken> RefreshTokens { get; set; }
        public IGenericRepository<Permission> Permissions { get; set; }

        Task<int> SaveChangesAsync();
    }
}