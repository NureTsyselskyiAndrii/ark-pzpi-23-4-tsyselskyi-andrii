using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SafeDose.Identity.Models;

namespace SafeDose.Identity.EntityConfigurations
{
    public class AuthUserConfiguration : IEntityTypeConfiguration<AuthUser>
    {
        public void Configure(EntityTypeBuilder<AuthUser> builder)
        {
            builder.HasKey(u => u.Id);

            builder.Property(u => u.Id)
                   .ValueGeneratedOnAdd();

            builder.Property(u => u.CreatedAt)
                   .IsRequired()
                   .HasDefaultValueSql("getdate()");

            builder.Property(u => u.Email)
                   .IsRequired()
                   .HasMaxLength(511);

            builder.Property(u => u.PhoneNumber)
                   .IsRequired(false)
                   .HasMaxLength(31);

            builder.Property(u => u.UserName)
                   .IsRequired()
                   .HasMaxLength(511);

            builder.HasIndex(u => u.UserName).IsUnique();

            builder.Property(u => u.BannedUntil)
                   .IsRequired()
                   .HasDefaultValueSql("'0001-01-01T00:00:00.000'");

            builder.Property(u => u.EmailConfirmationCode)
                   .IsRequired(false)
                   .HasMaxLength(15);

            builder.Property(u => u.EmailConfirmationCodeExpiryTime)
                   .IsRequired()
                   .HasDefaultValueSql("'0001-01-01T00:00:00.000'");

            builder.Property(u => u.PhoneConfirmationCode)
                   .IsRequired(false)
                   .HasMaxLength(15);

            builder.Property(u => u.PhoneConfirmationCodeExpiryTime)
                   .IsRequired()
                   .HasDefaultValueSql("'0001-01-01T00:00:00.000'");

            var hasher = new PasswordHasher<AuthUser>();

            var usersAuth = new List<AuthUser>
            {
                new AuthUser
                {
                    Id = 1L,
                    Email = "tsyselskyiandrey@gmail.com",
                    NormalizedEmail = "TSYSELSKYIANDREY@GMAIL.COM",
                    PhoneNumber = "+380682488040",
                    PhoneNumberConfirmed = true,
                    UserName = "Admin",
                    NormalizedUserName = "ADMIN",
                    PasswordHash = hasher.HashPassword(null, "Q1w2e3r4t5y6"),
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    CreatedAt = DateTime.UtcNow
                },
                new AuthUser
                {
                    Id = 2L,
                    Email = "john.doe@example.com",
                    NormalizedEmail = "JOHN.DOE@EXAMPLE.COM",
                    PhoneNumber = "+380501112233",
                    PhoneNumberConfirmed = true,
                    UserName = "JohnDoe",
                    NormalizedUserName = "JOHNDOE",
                    PasswordHash = hasher.HashPassword(null, "Q1w2e3r4t5y6"),
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    CreatedAt = DateTime.UtcNow
                },
                new AuthUser
                {
                    Id = 3L,
                    Email = "anna.smith@example.com",
                    NormalizedEmail = "ANNA.SMITH@EXAMPLE.COM",
                    PhoneNumber = "+380631234567",
                    PhoneNumberConfirmed = true,
                    UserName = "AnnaSmith",
                    NormalizedUserName = "ANNASMITH",
                    PasswordHash = hasher.HashPassword(null, "Q1w2e3r4t5y6"),
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    CreatedAt = DateTime.UtcNow
                },
                new AuthUser
                {
                    Id = 4L,
                    Email = "michael.brown@example.com",
                    NormalizedEmail = "MICHAEL.BROWN@EXAMPLE.COM",
                    PhoneNumber = "+380973456789",
                    PhoneNumberConfirmed = true,
                    UserName = "MikeBrown",
                    NormalizedUserName = "MIKEBROWN",
                    PasswordHash = hasher.HashPassword(null, "Q1w2e3r4t5y6"),
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    CreatedAt = DateTime.UtcNow
                },
                new AuthUser
                {
                    Id = 5L,
                    Email = "kate.jordan@example.com",
                    NormalizedEmail = "KATE.JORDAN@EXAMPLE.COM",
                    PhoneNumber = "+380992223344",
                    PhoneNumberConfirmed = true,
                    UserName = "KateJordan",
                    NormalizedUserName = "KATEJORDAN",
                    PasswordHash = hasher.HashPassword(null, "Q1w2e3r4t5y6"),
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    CreatedAt = DateTime.UtcNow
                }
            };

            builder.HasData(usersAuth);
        }
    }
}
