using System.Linq;
using PRN232.LMS.Identity.API.Infrastructure.Data;
using PRN232.LMS.Identity.API.Domain.Entities;
using BCrypt.Net;

namespace PRN232.LMS.Identity.API.Infrastructure.Data
{
    public static class DbSeeder
    {
        public static void Seed(IdentityDbContext context)
        {
            if (!context.Users.Any())
            {
                context.Users.Add(new User
                {
                    Username = "admin",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
                    Role = "Admin"
                });
                context.SaveChanges();
            }
        }
    }
}