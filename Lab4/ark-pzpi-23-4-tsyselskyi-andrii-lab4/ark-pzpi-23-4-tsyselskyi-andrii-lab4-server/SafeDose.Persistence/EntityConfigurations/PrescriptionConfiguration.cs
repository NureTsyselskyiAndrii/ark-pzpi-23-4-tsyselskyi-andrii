using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SafeDose.Domain.Entities;

namespace SafeDose.Persistence.EntityConfigurations
{
    public class PrescriptionConfiguration : IEntityTypeConfiguration<Prescription>
    {
        public void Configure(EntityTypeBuilder<Prescription> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(u => u.StartDate)
                   .IsRequired()
                   .HasDefaultValueSql("getdate()");

            builder.Property(u => u.EndDate)
                   .IsRequired()
                   .HasDefaultValueSql("getdate()");

            builder.Property(p => p.DoctorId)
                   .IsRequired();
            builder.HasOne(p => p.Doctor)
                   .WithMany(d => d.Prescriptions)
                   .HasForeignKey(p => p.DoctorId)
                   .OnDelete(DeleteBehavior.Cascade)
                   .IsRequired();

            builder.Property(p => p.PatientId)
                   .IsRequired();
            builder.HasOne(p => p.Patient)
                   .WithMany(p => p.Prescriptions)
                   .HasForeignKey(p => p.PatientId)
                   .OnDelete(DeleteBehavior.NoAction)  // ------ Attention here ------
                   .IsRequired();


            var prescriptions = new List<Prescription>
            {
                new Prescription
                {
                    Id = 1,
                    PatientId = 1,
                    DoctorId = 1,
                    StartDate = new DateTime(2024, 01, 10),
                    EndDate = new DateTime(2024, 02, 10)
                },
                new Prescription
                {
                    Id = 2,
                    PatientId = 2,
                    DoctorId = 2,
                    StartDate = new DateTime(2024, 03, 05),
                    EndDate = new DateTime(2024, 03, 20)
                }
            };

            builder.HasData(prescriptions);
        }
    }
}
