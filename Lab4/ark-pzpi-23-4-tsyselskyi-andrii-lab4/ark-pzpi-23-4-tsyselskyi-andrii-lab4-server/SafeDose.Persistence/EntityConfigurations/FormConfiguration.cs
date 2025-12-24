using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SafeDose.Domain.Entities;

namespace SafeDose.Persistence.EntityConfigurations
{
    public class FormConfiguration : IEntityTypeConfiguration<Form>
    {
        public void Configure(EntityTypeBuilder<Form> builder)
        {
            builder.HasKey(f => f.Id);

            builder.Property(f => f.Name)
                   .IsRequired()
                   .HasMaxLength(255);

            var forms = new List<Form>
            {
                new Form { Id = 1, Name = "Tablet" },
                new Form { Id = 2, Name = "Capsule" },
                new Form { Id = 3, Name = "Syrup" },
                new Form { Id = 4, Name = "Injection" },
                new Form { Id = 5, Name = "Ointment" }
            };

            builder.HasData(forms);
        }
    }
}
