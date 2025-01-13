namespace ChristmasGiftApi.Interfaces
{
    public interface INotificationService
    {
        Task SendEmailAsync(string to, string subject, string htmlContent);
        Task SendEmailsAsync(IEnumerable<string> to, string subject, string htmlContent);
    }
}
