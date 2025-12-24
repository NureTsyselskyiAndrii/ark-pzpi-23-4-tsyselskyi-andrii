using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SafeDose.Domain.Entities;

namespace SafeDose.Persistence.EntityConfigurations
{
    public class MedicationStockConfiguration : IEntityTypeConfiguration<MedicationStock>
    {
        public void Configure(EntityTypeBuilder<MedicationStock> builder)
        {
            builder.HasKey(ms => ms.Id);

            builder.Property(ms => ms.Quantity)
                   .IsRequired();

            builder.Property(ms => ms.ProductionDate)
                   .IsRequired()
                   .HasDefaultValueSql("getdate()");

            builder.Property(ms => ms.ExpirationDate)
                   .IsRequired()
                   .HasDefaultValueSql("getdate()");

            builder.Property(ms => ms.ReceivedAt)
                   .IsRequired()
                   .HasDefaultValueSql("getdate()");

            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_MedicationStock_Quantity_Positive", "[Quantity] > 0");
            });

            builder.Property(ms => ms.MedicationId)
                   .IsRequired();
            builder.HasOne(ms => ms.Medication)
                   .WithMany(m => m.MedicationStocks)
                   .HasForeignKey(ms => ms.MedicationId)
                   .OnDelete(DeleteBehavior.Cascade)
                   .IsRequired();

            builder.Property(ms => ms.WorkplaceId)
                   .IsRequired();
            builder.HasOne(ms => ms.Workplace)
                   .WithMany(wp => wp.MedicationStocks)
                   .HasForeignKey(ms => ms.WorkplaceId)
                   .OnDelete(DeleteBehavior.Cascade)
                   .IsRequired();

            var medicationStocks = new List<MedicationStock>
            {
                new MedicationStock
                {
                    Id = 1,
                    MedicationId = 1,
                    WorkplaceId = 1,
                    Quantity = 200,
                    ProductionDate = new DateTime(2024, 05, 01),
                    ExpirationDate = new DateTime(2027, 05, 01),
                    ReceivedAt = new DateTime(2024, 06, 10)
                },
                new MedicationStock
                {
                    Id = 2,
                    MedicationId = 2,
                    WorkplaceId = 1,
                    Quantity = 150,
                    ProductionDate = new DateTime(2024, 03, 15),
                    ExpirationDate = new DateTime(2027, 03, 15),
                    ReceivedAt = new DateTime(2024, 04, 01)
                },
                new MedicationStock
                {
                    Id = 3,
                    MedicationId = 3,
                    WorkplaceId = 1,
                    Quantity = 100,
                    ProductionDate = new DateTime(2024, 10, 10),
                    ExpirationDate = new DateTime(2027, 10, 10),
                    ReceivedAt = new DateTime(2024, 11, 05)
                },
                new MedicationStock
                {
                    Id = 4,
                    MedicationId = 4,
                    WorkplaceId = 1,
                    Quantity = 50,
                    ProductionDate = new DateTime(2023, 01, 20),
                    ExpirationDate = new DateTime(2027, 01, 20),
                    ReceivedAt = new DateTime(2024, 02, 10)
                },
                new MedicationStock
                {
                    Id = 5,
                    MedicationId = 5,
                    WorkplaceId = 1,
                    Quantity = 80,
                    ProductionDate = new DateTime(2024, 07, 12),
                    ExpirationDate = new DateTime(2027, 07, 12),
                    ReceivedAt = new DateTime(2024, 08, 01)
                }
            };

            builder.HasData(medicationStocks);
        }
    }
}
