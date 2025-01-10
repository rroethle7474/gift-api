// Services/NotificationService.cs
using ChristmasGiftApi.Data;
using ChristmasGiftApi.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ChristmasGiftApi.Services;

public class WishListNotificationService
{
    private readonly INotificationService _notificationService;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<WishListNotificationService> _logger;

    public WishListNotificationService(
        INotificationService notificationService,
        ApplicationDbContext context,
        ILogger<WishListNotificationService> logger)
    {
        _notificationService = notificationService;
        _context = context;
        _logger = logger;
    }

    public async Task SendWishListApprovalNotificationsAsync(int userId)
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
                string htmlContent = user?.Name != null ? $"{user.Name} has a wish list ready for approval" : "A wish list is ready for approval";
                //await _notificationService.SendEmailsAsync(emailRecipients, subject, htmlContent);
            }

            // Send SMS
            var phoneRecipients = new List<string>();
            if (!string.IsNullOrWhiteSpace(user.ParentPhone1))
                phoneRecipients.Add(user.ParentPhone1);
            if (!string.IsNullOrWhiteSpace(user.ParentPhone2))
                phoneRecipients.Add(user.ParentPhone2);

            phoneRecipients.Add("+12622247014"); // this is the default phone to send to


            if (phoneRecipients.Any())
            {
                // remove duplicates from phoneRecipients
                phoneRecipients = phoneRecipients.Distinct().ToList();
                string message = user?.Name != null ? $"{user.Name} has a wish list ready for approval" : "A wish list is ready for approval" ;
                await _notificationService.SendMultipleSmsAsync(phoneRecipients, message);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending wish list approval notifications for user {UserId}", userId);
            throw;
        }
    }
}