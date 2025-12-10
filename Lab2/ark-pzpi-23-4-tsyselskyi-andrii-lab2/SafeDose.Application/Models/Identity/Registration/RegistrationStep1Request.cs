namespace SafeDose.Application.Models.Identity.Registration
{
    public class RegistrationStep1Request
    {
        public string Email { get; set; } = String.Empty;
        public string UserName { get; set; } = String.Empty;
        public string Password { get; set; } = String.Empty;
        public string ConfirmPassword { get; set; } = String.Empty;
    }
}
