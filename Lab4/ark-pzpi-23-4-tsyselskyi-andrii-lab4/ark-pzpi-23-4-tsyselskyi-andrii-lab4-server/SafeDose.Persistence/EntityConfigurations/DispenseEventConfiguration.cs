using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SafeDose.Domain.Entities;

namespace SafeDose.Persistence.EntityConfigurations
{
    public class DispenseEventConfiguration : IEntityTypeConfiguration<DispenseEvent>
    {
        public void Configure(EntityTypeBuilder<DispenseEvent> builder)
        {
            builder.HasKey(de => de.Id);

            builder.Property(de => de.DispensedAt)
                   .IsRequired()
                   .HasDefaultValueSql("getdate()");

            builder.Property(de => de.QuantityDispensed)
                   .IsRequired();

            builder.Property(de => de.Price)
                   .IsRequired();

            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_DispenseEvent_QuantityDispensed_Positive", "[QuantityDispensed] >= 0");
                t.HasCheckConstraint("CK_DispenseEvent_Price_Positive", "[Price] >= 0");
            });

            builder.Property(de => de.PrescriptionId)
                   .IsRequired();
            builder.HasOne(de => de.Prescription)
                   .WithMany(p => p.DispenseEvents)
                   .HasForeignKey(de => de.PrescriptionId)
                   .OnDelete(DeleteBehavior.NoAction)  // ----- Attention -----
                   .IsRequired();

            builder.Property(de => de.DoctorId)
                   .IsRequired();
            builder.HasOne(de => de.Doctor)
                   .WithMany(d => d.DispenseEvents)
                   .HasForeignKey(de => de.DoctorId)
                   .OnDelete(DeleteBehavior.Cascade)
                   .IsRequired();

            builder.Property(de => de.PatientId)
                   .IsRequired();
            builder.HasOne(de => de.Patient)
                   .WithMany(p => p.DispenseEvents)
                   .HasForeignKey(de => de.PatientId)
                   .OnDelete(DeleteBehavior.NoAction)  // ----- Attention -----
                   .IsRequired();

            builder.Property(de => de.MedicationId)
                  .IsRequired();
            builder.HasOne(de => de.Medication)
                   .WithMany(m => m.DispenseEvents)
                   .HasForeignKey(de => de.MedicationId)
                   .OnDelete(DeleteBehavior.Cascade)
                   .IsRequired();

            builder.Property(de => de.DeviceId)
                   .IsRequired();
            builder.HasOne(de => de.Device)
                   .WithMany(d => d.DispenseEvents)
                   .HasForeignKey(de => de.DeviceId)
                   .OnDelete(DeleteBehavior.Cascade)
                   .IsRequired();
        }
    }
}
