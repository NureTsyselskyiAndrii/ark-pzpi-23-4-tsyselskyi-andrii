namespace SafeDose.API.Requests
{
    public class UpdatePrescriptionRequest
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public long PatientId { get; set; }
        public long DoctorId { get; set; }
    }
}
