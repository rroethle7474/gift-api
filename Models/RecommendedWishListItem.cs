using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChristmasGiftApi.Models;

public class RecommendWishListItem
{
    [Key]
    public int RecommendItemId { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    [MaxLength(100)]
    public string ItemName { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    [MaxLength(2048)]
    public string? ProductUrl { get; set; }
    [MaxLength(2048)]
    public string? ProductSrcImage { get; set; }

    public decimal? EstimatedCost { get; set; }

    [Required]
    public int DefaultQuantity { get; set; }

    [Required]
    public bool IsActive { get; set; }

    public DateTime DateAdded { get; set; }

    public DateTime LastModified { get; set; }

    [ForeignKey(nameof(UserId))]
    public User? User { get; set; }
}
