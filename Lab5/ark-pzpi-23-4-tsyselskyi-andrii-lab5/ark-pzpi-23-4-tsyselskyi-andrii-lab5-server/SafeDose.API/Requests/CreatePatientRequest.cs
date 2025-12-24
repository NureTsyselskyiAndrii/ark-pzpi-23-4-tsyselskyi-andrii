namespace SafeDose.API.Requests
{
    public class CreatePatientRequest
    {
        public string BloodType { get; set; } = null!;
        public long UserId { get; set; }
    }
}
