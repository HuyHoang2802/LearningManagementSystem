using System;
using System.Threading.Tasks;
using PRN232.LMS.Identity.API.Domain.Entities;
using PRN232.LMS.Identity.API.Infrastructure.Data;

namespace PRN232.LMS.Identity.API.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IdentityDbContext _context;
        public IGenericRepository<User> Users { get; set; }
        public IGenericRepository<RefreshToken> RefreshTokens { get; set; }
        public IGenericRepository<Permission> Permissions { get; set; }


        public UnitOfWork(
            IdentityDbContext context,
            IGenericRepository<User> users, IGenericRepository<RefreshToken> refreshtokens, IGenericRepository<Permission> permissions)
        {
            _context = context;
            Users = users;
            RefreshTokens = refreshtokens;
            Permissions = permissions;
        }

        public void Dispose() => _context.Dispose();
        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}