namespace SafeDose.Application.Models.Identity.General
{
    public class CompleteAuthResponse
    {
        public AuthResponse AuthResponse { get; set; }
        public RefreshTokenResponse RefreshTokenResponse { get; set; }
    }
}
