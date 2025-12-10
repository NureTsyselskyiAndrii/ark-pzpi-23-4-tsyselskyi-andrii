namespace SafeDose.API.Requests
{
    public class UpdateDoctorRequest
    {
        public string LicenseNumber { get; set; } = null!;
        public long SpecializationId { get; set; }
        public long WorkplaceId { get; set; }
        public long PositionId { get; set; }
        public long UserId { get; set; }
    }
}
