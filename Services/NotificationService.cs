// Services/NotificationService.cs
using Azure.Communication.Sms;
using ChristmasGiftApi.Interfaces;
using ChristmasGiftApi.Models;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace ChristmasGiftApi.Services;

public class NotificationService : INotificationService
{
    private readonly NotificationSettings _settings;
    private readonly ILogger<NotificationService> _logger;
    private readonly SendGridClient _sendGridClient;
    private readonly SmsClient _smsClient;

    public NotificationService(
        IOptions<NotificationSettings> settings,
        ILogger<NotificationService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
        _sendGridClient = new SendGridClient(_settings.SendGridApiKey);
        _smsClient = new SmsClient(_settings.AzureCommunicationServiceConnectionString);
    }

    public async Task SendEmailAsync(string to, string subject, string htmlContent)
    {
        try
        {
            var msg = CreateEmailMessage(new[] { to }, subject, htmlContent);
            var response = await _sendGridClient.SendEmailAsync(msg);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to send email to {To}. Status code: {StatusCode}",
                    to, response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending email to {To}", to);
            throw;
        }
    }

    public async Task SendEmailsAsync(IEnumerable<string> to, string subject, string htmlContent)
    {
        try
        {
            var msg = CreateEmailMessage(to, subject, htmlContent);
            var response = await _sendGridClient.SendEmailAsync(msg);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to send emails. Status code: {StatusCode}",
                    response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending multiple emails");
            throw;
        }
    }

    public async Task SendSmsAsync(string to, string message)
    {
        try
        {
            var response = await _smsClient.SendAsync(
                from: _settings.SmsFromNumber,
                to: to,
                message: message);

            if (response.Value.Successful)
            {
                _logger.LogInformation("SMS sent successfully to {To}", to);
            }
            else
            {
                _logger.LogError("Failed to send SMS to {To}", to);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending SMS to {To}", to);
            throw;
        }
    }

    public async Task SendMultipleSmsAsync(IEnumerable<string> to, string message)
    {
        var tasks = to.Select(phoneNumber => SendSmsAsync(phoneNumber, message));
        await Task.WhenAll(tasks);
    }

    private SendGridMessage CreateEmailMessage(IEnumerable<string> to, string subject, string htmlContent)
    {
        var msg = new SendGridMessage
        {
            From = new SendGrid.Helpers.Mail.EmailAddress(_settings.SendGridFromEmail, _settings.SendGridFromName),
            Subject = subject,
            HtmlContent = htmlContent
        };

        msg.AddTos(to.Select(email => new SendGrid.Helpers.Mail.EmailAddress(email)).ToList());
        return msg;
    }
}