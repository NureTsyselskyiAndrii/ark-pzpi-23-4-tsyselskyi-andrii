using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SafeDose.Domain.Entities;

namespace SafeDose.Persistence.EntityConfigurations
{
    public class PositionConfiguration : IEntityTypeConfiguration<Position>
    {
        public void Configure(EntityTypeBuilder<Position> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Name)
                   .IsRequired()
                   .HasMaxLength(255);

            var positions = new List<Position>
            {
                new Position { Id = 1, Name = "Senior Doctor" },
                new Position { Id = 2, Name = "Junior Doctor" },
                new Position { Id = 3, Name = "Consultant" },
                new Position { Id = 4, Name = "Intern" },
                new Position { Id = 5, Name = "Head of Department" }
            };
            builder.HasData(positions);
        }
    }
}
