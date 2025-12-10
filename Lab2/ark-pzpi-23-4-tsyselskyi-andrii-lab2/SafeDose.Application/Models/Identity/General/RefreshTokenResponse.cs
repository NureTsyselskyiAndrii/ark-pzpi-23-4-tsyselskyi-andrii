namespace SafeDose.Application.Models.Identity.General
{
    public class RefreshTokenResponse
    {
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiryTime { get; set; }
    }
}
