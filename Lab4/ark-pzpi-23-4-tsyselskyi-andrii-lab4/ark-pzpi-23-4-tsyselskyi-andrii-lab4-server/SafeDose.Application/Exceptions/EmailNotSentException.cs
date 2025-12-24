namespace SafeDose.Application.Exceptions
{
    public class EmailNotSentException : Exception
    {
        public EmailNotSentException(string message) : base(message) { }
    }
}
