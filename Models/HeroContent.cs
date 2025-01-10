using System.ComponentModel.DataAnnotations;

namespace ChristmasGiftApi.Models;

public class HeroContent
{
    [Key]
    public int ContentId { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string? Description { get; set; }
    
    public string? AnimationData { get; set; }
    
    public bool IsActive { get; set; }
    
    public DateTime CreatedDate { get; set; }
    public DateTime LastModifiedDate { get; set; }
} 