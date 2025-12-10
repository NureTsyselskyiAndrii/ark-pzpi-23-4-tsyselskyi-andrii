namespace SafeDose.Application.Models.Identity.Settings
{
    public class AuthSettings
    {
        public int CodeDurationInMinutes { get; set; }
        public int CodeResendDelayInSeconds { get; set; }
    }
}
