using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SafeDose.Domain.Entities;

namespace SafeDose.Persistence.EntityConfigurations
{
    public class ManufacturerConfiguration : IEntityTypeConfiguration<Manufacturer>
    {
        public void Configure(EntityTypeBuilder<Manufacturer> builder)
        {
            builder.HasKey(m => m.Id);

            builder.Property(m => m.Name)
                   .IsRequired()
                   .HasMaxLength(255);

            var manufacturers = new List<Manufacturer>
            {
                new Manufacturer { Id = 1, Name = "Pfizer" },
                new Manufacturer { Id = 2, Name = "Moderna" },
                new Manufacturer { Id = 3, Name = "Bayer" },
                new Manufacturer { Id = 4, Name = "Novartis" },
                new Manufacturer { Id = 5, Name = "Johnson & Johnson" }
            };
            builder.HasData(manufacturers);
        }
    }
}
