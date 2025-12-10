using Microsoft.AspNetCore.Identity;

namespace SafeDose.Identity.Models
{
    public class AuthUser : IdentityUser<long>
    {
        public DateTime BannedUntil { get; set; }
        public string? EmailConfirmationCode { get; set; }
        public DateTime EmailConfirmationCodeExpiryTime { get; set; }
        public string? PhoneConfirmationCode { get; set; }
        public DateTime PhoneConfirmationCodeExpiryTime { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}
