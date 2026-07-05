using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Identity.API.Domain.Entities;

namespace PRN232.LMS.Identity.API.Infrastructure.Data
{
    public class IdentityDbContext : DbContext
    {
        public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options)
        {
        }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<RefreshToken> RefreshTokens { get; set; }
        public virtual DbSet<Permission> Permissions { get; set; }

    }
}