namespace SafeDose.Application.Models.Identity.UserService
{
    public class UserModel
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string UserName { get; set; } = string.Empty;
        public bool EmailConfirmed { get; set; } = false;
        public bool PhoneNumberConfirmed { get; set; } = false;
        public string? Biography { get; set; }
        public string? AvatarUrl { get; set; }
        public DateTime? BirthDate { get; set; }
    }
}
