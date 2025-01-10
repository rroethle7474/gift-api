namespace ChristmasGiftApi.Interfaces
{
    public interface INotificationService
    {
        Task SendEmailAsync(string to, string subject, string htmlContent);
        Task SendEmailsAsync(IEnumerable<string> to, string subject, string htmlContent);
        Task SendSmsAsync(string to, string message);
        Task SendMultipleSmsAsync(IEnumerable<string> to, string message);
    }
}
