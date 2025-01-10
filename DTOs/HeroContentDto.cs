namespace ChristmasGiftApi.DTOs;

public class HeroContentDto
{
    public int ContentId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? AnimationData { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime LastModifiedDate { get; set; }
}

public class CreateHeroContentDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? AnimationData { get; set; }
    public bool IsActive { get; set; }
}

public class UpdateHeroContentDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? AnimationData { get; set; }
    public bool? IsActive { get; set; }
} 