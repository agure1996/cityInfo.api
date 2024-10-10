namespace CityInfo.API.Services
{
    /// <summary>
    /// A service for sending emails via a cloud-based mail provider.
    /// Implements the <see cref="IMailService"/> interface to provide email functionality.
    /// </summary>
    public class CloudMailService : IMailService
    {
        private string _mailTo = string.Empty;   // Recipient's email address
        private string _mailFrom = string.Empty;  // Sender's email address

        /// <summary>
        /// Initializes a new instance of the <see cref="CloudMailService"/> class.
        /// </summary>
        /// <param name="configuration">The application configuration settings.</param>
        public CloudMailService(IConfiguration configuration)
        {
            // Retrieve mail addresses from configuration settings
            _mailTo = configuration["mailSettings:mailToAddress"];
            _mailFrom = configuration["mailSettings:mailFromAddress"];
        }

        /// <summary>
        /// Sends an email with the specified subject and message.
        /// Currently, this method outputs the email details to the console instead of sending an actual email.
        /// </summary>
        /// <param name="subject">The subject of the email.</param>
        /// <param name="message">The body of the email.</param>
        public void Send(string subject, string message)
        {
            // Output email details to the console for demonstration purposes
            Console.WriteLine($"Mail from {_mailFrom} to {_mailTo}, with {nameof(CloudMailService)}.");
            Console.WriteLine($"Subject: {subject}");
            Console.WriteLine($"Message: {message}");
        }
    }
}
