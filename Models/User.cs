using System.ComponentModel.DataAnnotations;

namespace ChristmasGiftApi.Models;

public class User
{
    [Key]
    public int UserId { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [MaxLength(256)]
    public string PasswordHash { get; set; } = string.Empty;
    public string? OriginalPassword { get; set; }
    public string Name { get; set; } = string.Empty;

    public bool IsAdmin { get; set; }
    public decimal? SpendingLimit { get; set; }
    
    [MaxLength(500)]
    public string? SillyDescription { get; set; }
    public string? GreetingMessage { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Email { get; set; } = string.Empty;
    public string? ParentEmail1 { get; set; } = string.Empty;
    public string? ParentEmail2 { get; set; } = string.Empty;
    public string? ParentPhone1 { get; set; } = string.Empty;
    public string? ParentPhone2 { get; set; } = string.Empty;

    public DateTime CreatedDate { get; set; }
    public DateTime LastModifiedDate { get; set; }
    public DateTime? Birthday { get; set; }

    // Navigation properties
    public ICollection<WishListItem> WishListItems { get; set; } = new List<WishListItem>();
    public ICollection<WishListSubmission> WishListSubmissions { get; set; } = new List<WishListSubmission>();
} 