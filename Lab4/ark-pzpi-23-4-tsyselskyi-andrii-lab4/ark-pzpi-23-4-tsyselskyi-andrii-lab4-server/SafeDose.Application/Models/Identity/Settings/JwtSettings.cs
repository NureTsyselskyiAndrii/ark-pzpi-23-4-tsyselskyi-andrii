namespace SafeDose.Application.Models.Identity.Settings
{
    public class JwtSettings
    {
        public string Key { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public double AccessTokenValidityInMinutes { get; set; }
        public double RegistrationTokenValidityInMinutes { get; set; }
        public double RefreshTokenValidityInDays { get; set; }
    }
}
