namespace SafeDose.Application.Models.Identity.General
{
    public class AuthResponse
    {
        public long Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? AvatarUrl { get; set; }
        public IEnumerable<string> Roles { get; set; } = new List<string>();
        public string Token { get; set; } = string.Empty;
    }
}
