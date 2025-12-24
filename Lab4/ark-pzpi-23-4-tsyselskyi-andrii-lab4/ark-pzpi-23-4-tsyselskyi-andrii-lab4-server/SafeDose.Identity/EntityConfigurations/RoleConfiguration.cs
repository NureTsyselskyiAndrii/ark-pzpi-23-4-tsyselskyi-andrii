using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SafeDose.Identity.EntityConfigurations
{
    public class RoleConfiguration : IEntityTypeConfiguration<IdentityRole<long>>
    {
        public void Configure(EntityTypeBuilder<IdentityRole<long>> builder)
        {
            builder.HasData(
                new IdentityRole<long>
                {
                    Id = 1L,
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                },
                new IdentityRole<long>
                {
                    Id = 2L,
                    Name = "Doctor",
                    NormalizedName = "DOCTOR"
                },
                new IdentityRole<long>
                {
                    Id = 3L,
                    Name = "Patient",
                    NormalizedName = "PATIENT"
                }
            );
        }
    }
}
