using JWTAuthDotNet8.Entities;
using Microsoft.EntityFrameworkCore;

namespace JWTAuthDotNet8.Data
{
    public class UserDBContext(DbContextOptions<UserDBContext> options): DbContext(options)
    {
        public DbSet<User> Users { get; set; }
    }
}
