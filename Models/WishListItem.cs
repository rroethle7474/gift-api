using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChristmasGiftApi.Models;

public class WishListItem
{
    [Key]
    public int ItemId { get; set; }
    
    [Required]
    public int UserId { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string ItemName { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string? Description { get; set; }
    
    [Required]
    public int Quantity { get; set; } = 1;
    public decimal? EstimatedCost { get; set; }

    [MaxLength(2048)]
    public string? ProductUrl { get; set; }
    
    public DateTime DateAdded { get; set; }
    public DateTime LastModified { get; set; }
    
    // Navigation property
    [ForeignKey(nameof(UserId))]
    public User? User { get; set; }
} 