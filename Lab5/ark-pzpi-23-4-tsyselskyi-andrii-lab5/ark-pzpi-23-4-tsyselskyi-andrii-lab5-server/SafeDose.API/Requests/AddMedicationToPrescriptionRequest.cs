namespace SafeDose.API.Requests
{
    public class AddMedicationToPrescriptionRequest
    {
        public long MedicationId { get; set; }
        public decimal Dosage { get; set; }
        public int QuantityOfDosagesOverall { get; set; }
        public int PeriodInDays { get; set; }
        public string? Description { get; set; }
        public decimal Discount { get; set; }
    }
}
