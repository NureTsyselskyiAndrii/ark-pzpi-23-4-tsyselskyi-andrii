using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SafeDose.Domain.Entities;

namespace SafeDose.Persistence.EntityConfigurations
{
    public class PrescriptionMedicationConfiguration : IEntityTypeConfiguration<PrescriptionMedication>
    {
        public void Configure(EntityTypeBuilder<PrescriptionMedication> builder)
        {
            builder.HasKey(pm => pm.Id);

            builder.Property(pm => pm.Dosage)
                   .IsRequired();

            builder.Property(pm => pm.QuantityOfDosagesOverall)
                   .IsRequired();

            builder.Property(pm => pm.PeriodInDays)
                   .IsRequired();

            builder.Property(pm => pm.Description)
                   .IsRequired(false)
                   .HasMaxLength(4095);

            builder.Property(pm => pm.Discount)
                   .IsRequired()
                   .HasDefaultValue(0);

            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_PrescriptionMedication_Dosage_Positive", "[Dosage] > 0");
                t.HasCheckConstraint("CK_PrescriptionMedication_QuantityOfDosagesOverall_Positive", "[QuantityOfDosagesOverall] > 0");
                t.HasCheckConstraint("CK_PrescriptionMedication_PeriodInDays_Positive", "[PeriodInDays] > 0");
                t.HasCheckConstraint("CK_PrescriptionMedication_Discount_Positive", "[Discount] >= 0");
            });

            builder.Property(pm => pm.PrescriptionId)
                   .IsRequired();
            builder.HasOne(pm => pm.Prescription)
                   .WithMany(p => p.PrescriptionMedications)
                   .HasForeignKey(pm => pm.PrescriptionId)
                   .OnDelete(DeleteBehavior.Cascade)
                   .IsRequired();

            builder.Property(pm => pm.MedicationId)
                   .IsRequired();
            builder.HasOne(pm => pm.Medication)
                   .WithMany(m => m.PrescriptionMedications)
                   .HasForeignKey(pm => pm.MedicationId)
                   .OnDelete(DeleteBehavior.Cascade)
                   .IsRequired();

            var prescriptionMedications = new List<PrescriptionMedication>
            {
                new PrescriptionMedication
                {
                    Id = 1,
                    PrescriptionId = 1,
                    MedicationId = 1,
                    Dosage = 500,
                    QuantityOfDosagesOverall = 20,
                    PeriodInDays = 10,
                    Description = "Take one tablet twice a day after meals.",
                    Discount = 0.00m
                },
                new PrescriptionMedication
                {
                    Id = 2,
                    PrescriptionId = 2,
                    MedicationId = 2,
                    Dosage = 200,
                    QuantityOfDosagesOverall = 15,
                    PeriodInDays = 5,
                    Description = "Use for pain relief when necessary.",
                    Discount = 10.00m
                }
            };

            builder.HasData(prescriptionMedications);
        }
    }
}
