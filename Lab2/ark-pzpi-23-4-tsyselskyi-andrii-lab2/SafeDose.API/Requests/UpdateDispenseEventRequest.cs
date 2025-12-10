namespace SafeDose.API.Requests
{
    public class UpdateDispenseEventRequest
    {
        public int QuantityDispensed { get; set; }
        public DateTime? DispensedAt { get; set; }
        public decimal Price { get; set; }

        public long PrescriptionId { get; set; }
        public long DeviceId { get; set; }
        public long PatientId { get; set; }
        public long DoctorId { get; set; }
        public long MedicationId { get; set; }
    }
}
