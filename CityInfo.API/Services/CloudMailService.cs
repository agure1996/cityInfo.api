namespace CityInfo.API.Services
{
    public class CloudMailService : IMailService
    {

        private string _mailTo = string.Empty;
        private string _mailFrom = string.Empty;

        public CloudMailService(IConfiguration configuration)
        {
            _mailTo = configuration["mailSettings:mailToAddress"];
            _mailFrom = configuration["mailSettings:mailFromAddress"];
        }
        public void send(string subject, string message)
        {
            //sending email - output to console window
            Console.WriteLine($"Mail from {_mailFrom} to {_mailTo}, with {nameof(CloudMailService)}.");
            Console.WriteLine($"Subject: {subject}");
            Console.WriteLine($"Message: {message}");
        }
    }
}
