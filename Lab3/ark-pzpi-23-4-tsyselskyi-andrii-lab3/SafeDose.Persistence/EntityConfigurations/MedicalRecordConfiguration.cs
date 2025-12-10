using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SafeDose.Domain.Entities;

namespace SafeDose.Persistence.EntityConfigurations
{
    public class MedicalRecordConfiguration : IEntityTypeConfiguration<MedicalRecord>
    {
        public void Configure(EntityTypeBuilder<MedicalRecord> builder)
        {
            builder.HasKey(mr => mr.Id);

            builder.Property(mr => mr.Description)
                   .IsRequired()
                   .HasMaxLength(4095);

            builder.Property(mr => mr.PatientId)
                   .IsRequired();
            builder.HasOne(mr => mr.Patient)
                   .WithMany(p => p.MedicalRecords)
                   .HasForeignKey(mr => mr.PatientId)
                   .OnDelete(DeleteBehavior.Cascade)
                   .IsRequired();

            var medicalRecords = new List<MedicalRecord>
            {
                new MedicalRecord
                {
                    Id = 1,
                    PatientId = 1,
                    Description = "General check-up. Slight anemia noted. Recommended vitamins."
                },
                new MedicalRecord
                {
                    Id = 2,
                    PatientId = 2,
                    Description = "Routine examination. No abnormalities detected."
                }
            };

            builder.HasData(medicalRecords);
        }
    }
}
