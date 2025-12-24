namespace SafeDose.Application.Models.Email
{
    public class EmailMessage
    {
        public string To { get; set; } = String.Empty;
        public string Subject { get; set; } = String.Empty;
        public string Body { get; set; } = String.Empty;
        public bool IsBodyHtml { get; set; }
    }
}
