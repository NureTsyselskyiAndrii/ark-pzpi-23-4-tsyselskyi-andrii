namespace SafeDose.API.Requests
{
    public class UpdateDeviceRequest
    {
        public string Name { get; set; } = null!;
        public long WorkplaceId { get; set; }
    }
}
