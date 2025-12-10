namespace SafeDose.Application.Models.Identity.GoogleAuth
{
    public class GoogleAuthRequest
    {
        public string CodeOrIdToken { get; set; } = String.Empty;
        public string DeviceId { get; set; } = String.Empty;
        public bool IsMobile { get; set; } = false;
    }
}
