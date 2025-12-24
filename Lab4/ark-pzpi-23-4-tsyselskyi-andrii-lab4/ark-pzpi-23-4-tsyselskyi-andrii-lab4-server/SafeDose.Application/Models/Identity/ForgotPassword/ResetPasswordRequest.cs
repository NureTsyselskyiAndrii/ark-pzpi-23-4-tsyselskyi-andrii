namespace SafeDose.Application.Models.Identity.ForgotPassword
{
    public class ResetPasswordRequest
    {
        public string Password { get; set; } = String.Empty;
        public string ConfirmPassword { get; set; } = String.Empty;
        public string Email { get; set; } = String.Empty;
        public string Token { get; set; } = String.Empty;
    }
}
