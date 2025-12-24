namespace SafeDose.Application.Models.Identity.LogIn
{
    public class LogInRequest
    {
        public string Login { get; set; } = String.Empty;
        public string Password { get; set; } = String.Empty;
        public string DeviceId { get; set; } = String.Empty;
    }
}
