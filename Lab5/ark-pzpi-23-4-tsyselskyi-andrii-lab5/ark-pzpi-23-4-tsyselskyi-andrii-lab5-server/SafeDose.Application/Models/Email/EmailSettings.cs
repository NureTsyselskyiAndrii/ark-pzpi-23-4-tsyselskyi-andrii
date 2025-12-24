namespace SafeDose.Application.Models.Email
{
    public class EmailSettings
    {
        public string SmtpServer { get; set; } = String.Empty;
        public int Port { get; set; }
        public string SenderEmail { get; set; } = String.Empty;
        public string SenderPassword { get; set; } = String.Empty;
        public string SenderName { get; set; } = String.Empty;
    }
}
