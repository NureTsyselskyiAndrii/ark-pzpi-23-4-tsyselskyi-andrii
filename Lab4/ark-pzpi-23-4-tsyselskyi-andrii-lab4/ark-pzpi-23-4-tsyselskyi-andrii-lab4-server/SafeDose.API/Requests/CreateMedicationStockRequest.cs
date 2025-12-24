namespace SafeDose.API.Requests
{
    public class CreateMedicationStockRequest
    {
        public int Quantity { get; set; }
        public DateTime ProductionDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public DateTime ReceivedAt { get; set; }
        public long WorkplaceId { get; set; }
        public long MedicationId { get; set; }
    }
}
