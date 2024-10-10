namespace CityInfo.API.Services
{
    /// <summary>
    /// Represents a local email service for sending emails during development or testing.
    /// Implements the <see cref="IMailService"/> interface.
    /// </summary>
    public class LocalMailService : IMailService
    {
        private string _mailTo = string.Empty;  // Recipient's email address
        private string _mailFrom = string.Empty; // Sender's email address

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalMailService"/> class.
        /// </summary>
        /// <param name="configuration">An instance of <see cref="IConfiguration"/> to retrieve mail settings.</param>
        public LocalMailService(IConfiguration configuration)
        {
            // Retrieve mail addresses from configuration settings
            _mailTo = configuration["mailSettings:mailToAddress"];
            _mailFrom = configuration["mailSettings:mailFromAddress"];
        }

        /// <summary>
        /// Sends an email with the specified subject and message.
        /// This method simulates sending an email by outputting details to the console.
        /// </summary>
        /// <param name="subject">The subject of the email.</param>
        /// <param name="message">The body of the email.</param>
        public void Send(string subject, string message)
        {
            // Output email details to the console for demonstration purposes
            Console.WriteLine($"Mail from {_mailFrom} to {_mailTo}, with {nameof(LocalMailService)}.");
            Console.WriteLine($"Subject: {subject}");
            Console.WriteLine($"Message: {message}");
        }
    }
}
