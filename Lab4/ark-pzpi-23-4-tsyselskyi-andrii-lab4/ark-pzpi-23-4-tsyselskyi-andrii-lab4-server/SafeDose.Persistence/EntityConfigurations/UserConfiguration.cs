using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SafeDose.Domain.Entities;

namespace SafeDose.Persistence.EntityConfigurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.Id);

            builder.Property(u => u.Id)
                   .ValueGeneratedNever();

            builder.Property(x => x.FirstName)
                   .IsRequired()
                   .HasMaxLength(255);

            builder.Property(x => x.LastName)
                   .IsRequired()
                   .HasMaxLength(255);

            builder.Property(x => x.Email)
                   .IsRequired()
                   .HasMaxLength(511);

            builder.HasIndex(u => u.Email)
                   .IsUnique();

            builder.Property(x => x.UserName)
                   .IsRequired()
                   .HasMaxLength(511);

            builder.HasIndex(u => u.UserName)
                   .IsUnique();

            builder.Property(u => u.CreatedDate)
                   .IsRequired()
                   .HasDefaultValueSql("getdate()");

            builder.Property(c => c.Biography)
                   .IsRequired(false)
                   .HasMaxLength(1023);

            builder.Property(c => c.BirthDate)
                   .IsRequired(false);

            builder.Property(c => c.PhoneNumber)
                   .IsRequired(false)
                   .HasMaxLength(31);

            builder.Property(c => c.AvatarUrl)
                   .IsRequired(false)
                   .HasMaxLength(511);

            var users = new List<User>
            {
                new User
                {
                    Id = 1L,
                    FirstName = "Andrey",
                    LastName = "Tsyselskyi",
                    UserName = "Admin",
                    Email = "tsyselskyiandrey@gmail.com",
                    PhoneNumber = "+380682488040",
                    CreatedDate = DateTime.UtcNow,
                    Biography = "System administrator",
                    BirthDate = new DateTime(1997, 01, 01)
                },
                new User
                {
                    Id = 2L,
                    FirstName = "John",
                    LastName = "Doe",
                    UserName = "JohnDoe",
                    Email = "john.doe@example.com",
                    PhoneNumber = "+380501112233",
                    CreatedDate = DateTime.UtcNow,
                    Biography = "Experienced surgeon",
                    BirthDate = new DateTime(1990, 05, 10)
                },
                new User
                {
                    Id = 3L,
                    FirstName = "Anna",
                    LastName = "Smith",
                    UserName = "AnnaSmith",
                    Email = "anna.smith@example.com",
                    PhoneNumber = "+380631234567",
                    CreatedDate = DateTime.UtcNow,
                    Biography = "Caring nurse",
                    BirthDate = new DateTime(1995, 10, 15)
                },
                new User
                {
                    Id = 4L,
                    FirstName = "Michael",
                    LastName = "Brown",
                    UserName = "MikeBrown",
                    Email = "michael.brown@example.com",
                    PhoneNumber = "+380973456789",
                    CreatedDate = DateTime.UtcNow,
                    Biography = "Fitness fan & healthy lifestyle supporter",
                    BirthDate = new DateTime(1988, 03, 22)
                },
                new User
                {
                    Id = 5L,
                    FirstName = "Kate",
                    LastName = "Jordan",
                    UserName = "KateJordan",
                    Email = "kate.jordan@example.com",
                    PhoneNumber = "+380992223344",
                    CreatedDate = DateTime.UtcNow,
                    Biography = "Designer & creative thinker",
                    BirthDate = new DateTime(1999, 12, 01)
                }
            };

            builder.HasData(users);
        }
    }
}
