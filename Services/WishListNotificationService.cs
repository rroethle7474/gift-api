// Services/NotificationService.cs
using ChristmasGiftApi.Data;
using ChristmasGiftApi.Interfaces;
using ChristmasGiftApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ChristmasGiftApi.Services;

public class WishListNotificationService
{
    private readonly INotificationService _notificationService;
    private readonly NotificationSettings _settings;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<WishListNotificationService> _logger;

    public WishListNotificationService(
        INotificationService notificationService,
        IOptions<NotificationSettings> settings,
        ApplicationDbContext context,
        ILogger<WishListNotificationService> logger)
    {
        _notificationService = notificationService;
        _settings = settings.Value;
        _context = context;
        _logger = logger;
    }

    public async Task SendWishListApprovalNotificationsAsync(int userId, int submissionId)
    {
        try
        {
            // Get user with parent contact info
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return;

            // Get admin emails
            var adminEmails = await _context.Users
                .Where(u => u.IsAdmin)
                .Select(u => u.Email)
                .ToListAsync();

            // Combine all email recipients
            var emailRecipients = new List<string>();
            if (!string.IsNullOrWhiteSpace(user.ParentEmail1))
                emailRecipients.Add(user.ParentEmail1);
            if (!string.IsNullOrWhiteSpace(user.ParentEmail2))
                emailRecipients.Add(user.ParentEmail2);
            emailRecipients.AddRange(adminEmails);

            // Send emails
            if (emailRecipients.Any())
            {
                string subject = "Wish List Approval Required";
                string approvalLink = _settings.BaseApprovalUrl + userId + "/" + submissionId;

                // Updated HTML content with the new template
                string htmlContent = $@"
                <!DOCTYPE html>
                <html lang='en'>
                <head>
                    <meta charset='UTF-8'>
                    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                    <title>Wish List Approval</title>
                    <style>
                        body {{
                            font-family: Arial, sans-serif;
                            background-color: #f4f4f4;
                            margin: 0;
                            padding: 0;
                        }}
                        .email-container {{
                            max-width: 600px;
                            margin: 0 auto;
                            background-color: #ffffff;
                            padding: 20px;
                            border-radius: 8px;
                            box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
                        }}
                        .header {{
                            font-size: 24px;
                            color: #333333;
                            margin-bottom: 20px;
                        }}
                        .content {{
                            font-size: 16px;
                            color: #555555;
                            line-height: 1.6;
                        }}
                        .button {{
                            display: inline-block;
                            padding: 10px 20px;
                            margin-top: 20px;
                            font-size: 16px;
                            color: #ffffff;
                            background-color: #007bff;
                            text-decoration: none;
                            border-radius: 5px;
                        }}
                        .footer {{
                            margin-top: 30px;
                            font-size: 14px;
                            color: #888888;
                            text-align: center;
                        }}
                    </style>
                </head>
                <body>
                    <div class='email-container'>
                        <div class='header'>Wish List Approval Required</div>
                        <div class='content'>
                            <p>Hello,</p>
                            <p>{(user?.Name != null ? $"{user.Name} has a wish list ready for approval." : "A wish list is ready for approval.")}</p>
                            <p>Please visit the link below to review the submission:</p>
                            <a href='{approvalLink}' class='button'>Review Submission</a>
                            <p>If the button above doesn't work, you can also copy and paste the following link into your browser:</p>
                            <p><a href='{approvalLink}'>{approvalLink}</a></p>
                        </div>
                        <div class='footer'>
                            <p>Thank you,</p>
                            <p>Your Company Name</p>
                        </div>
                    </div>
                </body>
                </html>
                ";

                await _notificationService.SendEmailsAsync(emailRecipients, subject, htmlContent);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending wish list approval notifications for user {UserId}", userId);
            throw;
        }
    }
}