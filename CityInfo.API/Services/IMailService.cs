namespace CityInfo.API.Services
{
    /// <summary>
    /// Represents a service for sending emails.
    /// Implementations of this interface should provide the necessary logic to send emails.
    /// </summary>
    public interface IMailService
    {
        /// <summary>
        /// Sends an email with the specified subject and message.
        /// </summary>
        /// <param name="subject">The subject of the email.</param>
        /// <param name="message">The body content of the email.</param>
        void Send(string subject, string message);
    }
}
