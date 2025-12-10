using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SafeDose.Identity.Models;

namespace SafeDose.Identity.DbContexts
{
    public class AuthenticationDbContext : IdentityDbContext<AuthUser, IdentityRole<long>, long>
    {
        public AuthenticationDbContext(DbContextOptions<AuthenticationDbContext> options)
            : base(options)
        {

        }

        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AuthenticationDbContext).Assembly);
            modelBuilder.HasDefaultSchema("authentication_schema");

            modelBuilder.Entity<IdentityUserRole<long>>().HasData(
                new IdentityUserRole<long>
                {
                    UserId = 1L,
                    RoleId = 1L
                },
                new IdentityUserRole<long>
                {
                    UserId = 2L,
                    RoleId = 2L
                },
                new IdentityUserRole<long>
                {
                    UserId = 3L,
                    RoleId = 2L
                },
                new IdentityUserRole<long>
                {
                    UserId = 4L,
                    RoleId = 3L
                },
                new IdentityUserRole<long>
                {
                    UserId = 5L,
                    RoleId = 3L
                }
            );
        }
    }
}
