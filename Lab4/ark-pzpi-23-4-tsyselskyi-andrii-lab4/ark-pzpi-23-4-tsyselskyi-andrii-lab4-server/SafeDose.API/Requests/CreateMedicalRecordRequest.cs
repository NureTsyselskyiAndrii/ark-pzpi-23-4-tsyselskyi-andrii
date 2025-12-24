namespace SafeDose.API.Requests
{
    public class CreateMedicalRecordRequest
    {
        public string Description { get; set; } = null!;
        public long PatientId { get; set; }
    }
}
