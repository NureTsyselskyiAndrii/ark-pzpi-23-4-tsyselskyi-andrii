namespace SafeDose.Application.Models.Identity.Settings
{
    public class GoogleSettings
    {
        public string ClientSecret { get; set; } = String.Empty;
        public string RedirectUri { get; set; } = String.Empty;
        public string WebClientId { get; set; } = String.Empty;
        public string AndroidClientId { get; set; } = String.Empty;
        public string IOSClientId { get; set; } = String.Empty;
    }
}
