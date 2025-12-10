using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SafeDose.Domain.Entities;

namespace SafeDose.Persistence.EntityConfigurations
{
    public class PatientConfiguration : IEntityTypeConfiguration<Patient>
    {
        public void Configure(EntityTypeBuilder<Patient> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.BloodType)
                   .IsRequired()
                   .HasMaxLength(31);

            builder.Property(p => p.UserId)
                   .IsRequired();
            builder.HasIndex(p => p.UserId, "IX_Patients_UserId")
                   .IsUnique();
            builder.HasOne(p => p.User)
                   .WithOne(u => u.Patient)
                   .HasForeignKey<Patient>(p => p.UserId)
                   .OnDelete(DeleteBehavior.Cascade)
                   .IsRequired();

            var patients = new List<Patient>
            {
                new Patient
                {
                    Id = 1,
                    UserId = 4,
                    BloodType = "A+"
                },
                new Patient
                {
                    Id = 2,
                    UserId = 5,
                    BloodType = "O-"
                }
            };
            builder.HasData(patients);
        }
    }
}
