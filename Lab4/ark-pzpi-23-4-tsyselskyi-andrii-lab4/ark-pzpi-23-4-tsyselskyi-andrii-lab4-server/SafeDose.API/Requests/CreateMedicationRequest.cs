namespace SafeDose.API.Requests
{
    public class CreateMedicationRequest
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
        public long FormId { get; set; }
    }

    public class UpdateMedicationRequest : CreateMedicationRequest { }

}
