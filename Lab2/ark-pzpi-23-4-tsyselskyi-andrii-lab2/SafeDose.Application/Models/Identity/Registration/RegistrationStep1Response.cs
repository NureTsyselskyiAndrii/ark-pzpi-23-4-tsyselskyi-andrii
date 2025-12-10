namespace SafeDose.Application.Models.Identity.Registration
{
    public class RegistrationStep1Response
    {
        public string Token { get; set; } = String.Empty;
        public DateTime ExpiryTime { get; set; }
    }
}
