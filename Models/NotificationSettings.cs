namespace ChristmasGiftApi.Models
{
    public class NotificationSettings
    {
        public string SendGridApiKey { get; set; } = string.Empty;
        public string SendGridFromEmail { get; set; } = string.Empty;
        public string SendGridFromName { get; set; } = string.Empty;
        public string AzureCommunicationServiceConnectionString { get; set; } = string.Empty;
        public string SmsFromNumber { get; set; } = string.Empty;
    }
}
