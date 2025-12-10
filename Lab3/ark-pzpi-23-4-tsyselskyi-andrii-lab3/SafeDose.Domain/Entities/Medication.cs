using SafeDose.Domain.Common;

namespace SafeDose.Domain.Entities
{
    public class Medication : BaseEntity<long>
    {
        public string Name { get; set; } = null!;
        public string Barcode { get; set; } = null!;
        public string? Description { get; set; }
        public string? StorageConditions { get; set; }
        public string? Contraindications { get; set; }
        public string? SideEffects { get; set; }
        public decimal StrengthAmount { get; set; }
        public string StrengthUnit { get; set; } = null!;
        public string StrengthBase { get; set; } = null!;
        public decimal VolumePerBlister { get; set; }
        public decimal VolumePerPackage { get; set; }
        public string VolumeUnit { get; set; } = null!;
        public decimal PricePerBlister { get; set; }
        public string? ImageUrl { get; set; }

        public long ManufacturerId { get; set; }
        public virtual Manufacturer Manufacturer { get; set; } = null!;

        public long FormId { get; set; }
        public virtual Form Form { get; set; } = null!;

        public virtual ICollection<PrescriptionMedication> PrescriptionMedications { get; set; } = new List<PrescriptionMedication>();
        public virtual ICollection<DispenseEvent> DispenseEvents { get; set; } = new List<DispenseEvent>();
        public virtual ICollection<MedicationStock> MedicationStocks { get; set; } = new List<MedicationStock>();
    }
}
