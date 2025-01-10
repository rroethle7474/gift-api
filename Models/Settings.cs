using System.ComponentModel.DataAnnotations;

namespace ChristmasGiftApi.Models;

public class Setting
{
    [Key]
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public string? Value { get; set; }
}