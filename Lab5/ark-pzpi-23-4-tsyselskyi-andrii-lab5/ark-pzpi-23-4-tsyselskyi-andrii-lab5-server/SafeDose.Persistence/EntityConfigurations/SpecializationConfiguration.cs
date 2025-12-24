using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SafeDose.Domain.Entities;

namespace SafeDose.Persistence.EntityConfigurations
{
    public class SpecializationConfiguration : IEntityTypeConfiguration<Specialization>
    {
        public void Configure(EntityTypeBuilder<Specialization> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.Name)
                   .IsRequired()
                   .HasMaxLength(255);

            var specializations = new List<Specialization>
            {
                new Specialization { Id = 1, Name = "Cardiologist" },
                new Specialization { Id = 2, Name = "Neurologist" },
                new Specialization { Id = 3, Name = "Dermatologist" },
                new Specialization { Id = 4, Name = "Pediatrician" },
                new Specialization { Id = 5, Name = "General Physician" }
            };
            builder.HasData(specializations);
        }
    }
}
