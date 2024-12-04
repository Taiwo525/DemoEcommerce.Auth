using Auth.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Auth.Infrastructure.Persistence
{
    public class AuthDbContext(DbContextOptions<AuthDbContext> options) : DbContext(options)
    {
        public DbSet<AppUser> AppUsers { get; set; }
    }
}
