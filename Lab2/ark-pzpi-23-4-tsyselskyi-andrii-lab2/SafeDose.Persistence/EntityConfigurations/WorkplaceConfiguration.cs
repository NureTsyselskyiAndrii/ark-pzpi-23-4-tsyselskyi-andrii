using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SafeDose.Domain.Entities;

namespace SafeDose.Persistence.EntityConfigurations
{
    public class WorkplaceConfiguration : IEntityTypeConfiguration<Workplace>
    {
        public void Configure(EntityTypeBuilder<Workplace> builder)
        {
            builder.HasKey(wp => wp.Id);

            builder.Property(wp => wp.Name)
                   .IsRequired()
                   .HasMaxLength(255);

            builder.Property(wp => wp.Address)
                   .IsRequired()
                   .HasMaxLength(511);

            var workplaces = new List<Workplace>
            {
                new Workplace { Id = 1, Name = "City Hospital #1", Address = "123 Main St, Kyiv" },
                new Workplace { Id = 2, Name = "Private Clinic 'HealthPlus'", Address = "45 Green Ave, Lviv" },
                new Workplace { Id = 3, Name = "Regional Medical Center", Address = "81 Central Rd, Odesa" },
                new Workplace { Id = 4, Name = "Family Clinic 'DobroMed'", Address = "14 Peace St, Dnipro" },
                new Workplace { Id = 5, Name = "Diagnostic Center 'MedExpert'", Address = "27 Horizon Blvd, Kharkiv" }
            };
            builder.HasData(workplaces);
        }
    }
}
