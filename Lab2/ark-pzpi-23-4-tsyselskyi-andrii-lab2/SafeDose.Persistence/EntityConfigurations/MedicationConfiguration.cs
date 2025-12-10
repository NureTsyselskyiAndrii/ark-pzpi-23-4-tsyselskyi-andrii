using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SafeDose.Domain.Entities;

namespace SafeDose.Persistence.EntityConfigurations
{
    public class MedicationConfiguration : IEntityTypeConfiguration<Medication>
    {
        public void Configure(EntityTypeBuilder<Medication> builder)
        {
            builder.HasKey(m => m.Id);

            builder.Property(m => m.Name)
                   .IsRequired()
                   .HasMaxLength(255);

            builder.Property(m => m.Barcode)
                   .IsRequired()
                   .HasMaxLength(255);

            builder.HasIndex(m => m.Barcode).IsUnique();

            builder.Property(m => m.Description)
                   .IsRequired(false)
                   .HasMaxLength(4095);

            builder.Property(m => m.StorageConditions)
                  .IsRequired(false)
                  .HasMaxLength(4095);

            builder.Property(m => m.Contraindications)
                  .IsRequired(false)
                  .HasMaxLength(4095);

            builder.Property(m => m.SideEffects)
                  .IsRequired(false)
                  .HasMaxLength(4095);

            builder.Property(m => m.StrengthAmount)
                   .IsRequired();

            builder.Property(m => m.StrengthUnit)
                  .IsRequired()
                  .HasMaxLength(31);

            builder.Property(m => m.StrengthBase)
                  .IsRequired()
                  .HasMaxLength(63);

            builder.Property(m => m.VolumePerBlister)
                  .IsRequired();

            builder.Property(m => m.VolumePerPackage)
                  .IsRequired();

            builder.Property(m => m.VolumeUnit)
                  .IsRequired()
                  .HasMaxLength(31);

            builder.Property(m => m.PricePerBlister)
                  .IsRequired();

            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_Medication_StrengthAmount_Positive", "[StrengthAmount] > 0");
                t.HasCheckConstraint("CK_Medication_VolumePerBlister_Positive", "[VolumePerBlister] >= 0");
                t.HasCheckConstraint("CK_Medication_VolumePerPackage_Positive", "[VolumePerPackage] >= 0");
                t.HasCheckConstraint("CK_Medication_PricePerBlister_Positive", "[PricePerBlister] >= 0");
            });

            builder.Property(m => m.ImageUrl)
                   .IsRequired(false)
                   .HasMaxLength(511);

            builder.Property(m => m.ManufacturerId)
                   .IsRequired();
            builder.HasOne(m => m.Manufacturer)
                   .WithMany(m => m.Medications)
                   .HasForeignKey(m => m.ManufacturerId)
                   .OnDelete(DeleteBehavior.Restrict)
                   .IsRequired();

            builder.Property(m => m.FormId)
                   .IsRequired();
            builder.HasOne(m => m.Form)
                   .WithMany(f => f.Medications)
                   .HasForeignKey(m => m.FormId)
                   .OnDelete(DeleteBehavior.Restrict)
                   .IsRequired();

            var medications = new List<Medication>
            {
                new Medication
                {
                    Id = 1,
                    Name = "Paracetamol",
                    Barcode = "1234567890123",
                    Description = "Pain reliever and fever reducer.",
                    StorageConditions = "Store at room temperature.",
                    Contraindications = "Liver disease.",
                    SideEffects = "Nausea, dizziness.",
                    StrengthAmount = 500,
                    StrengthUnit = "mg",
                    StrengthBase = "per tablet",
                    VolumePerBlister = 12,
                    VolumePerPackage = 5,
                    VolumeUnit = "tablets",
                    PricePerBlister = 2.50m,
                    ManufacturerId = 3,
                    FormId = 1
                },
                new Medication
                {
                    Id = 2,
                    Name = "Ibuprofen",
                    Barcode = "9876543210987",
                    Description = "Anti-inflammatory and analgesic.",
                    StorageConditions = "Store in dry place.",
                    Contraindications = "Ulcer, kidney disease.",
                    SideEffects = "Stomach upset.",
                    StrengthAmount = 200,
                    StrengthUnit = "mg",
                    StrengthBase = "per tablet",
                    VolumePerBlister = 10,
                    VolumePerPackage = 4,
                    VolumeUnit = "tablets",
                    PricePerBlister = 3.00m,
                    ManufacturerId = 3,
                    FormId = 1
                },
                new Medication
                {
                    Id = 3,
                    Name = "Omeprazole",
                    Barcode = "5551237770001",
                    Description = "Reduces stomach acid.",
                    StorageConditions = "Keep away from sunlight.",
                    Contraindications = "Pregnancy unless prescribed.",
                    SideEffects = "Headache, diarrhea.",
                    StrengthAmount = 20,
                    StrengthUnit = "mg",
                    StrengthBase = "per capsule",
                    VolumePerBlister = 7,
                    VolumePerPackage = 4,
                    VolumeUnit = "capsules",
                    PricePerBlister = 4.20m,
                    ManufacturerId = 4,
                    FormId = 2
                },
                new Medication
                {
                    Id = 4,
                    Name = "Azithromycin",
                    Barcode = "2223334445556",
                    Description = "Antibiotic for bacterial infections.",
                    StorageConditions = "Store below 25°C.",
                    Contraindications = "Heart rhythm disorders.",
                    SideEffects = "Diarrhea, abdominal pain.",
                    StrengthAmount = 500,
                    StrengthUnit = "mg",
                    StrengthBase = "per tablet",
                    VolumePerBlister = 3,
                    VolumePerPackage = 3,
                    VolumeUnit = "tablets",
                    PricePerBlister = 5.70m,
                    ManufacturerId = 5,
                    FormId = 1
                },
                new Medication
                {
                    Id = 5,
                    Name = "Dexpanthenol",
                    Barcode = "1119993337770",
                    Description = "Skin regeneration ointment.",
                    StorageConditions = "Store below 25°C.",
                    Contraindications = "Hypersensitivity.",
                    SideEffects = "Local irritation.",
                    StrengthAmount = 5,
                    StrengthUnit = "%",
                    StrengthBase = "ointment",
                    VolumePerBlister = 1,
                    VolumePerPackage = 1,
                    VolumeUnit = "tube",
                    PricePerBlister = 6.00m,
                    ManufacturerId = 2,
                    FormId = 5
                }
            };
            builder.HasData(medications);
        }
    }
}
