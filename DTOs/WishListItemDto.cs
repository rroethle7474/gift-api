namespace ChristmasGiftApi.DTOs;

public class WishListItemDto
{
    public int ItemId { get; set; }
    public int UserId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Quantity { get; set; }
    public decimal? EstimatedCost { get; set; }
    public string? ProductUrl { get; set; }
    public DateTime DateAdded { get; set; }
    public DateTime LastModified { get; set; }
}

public class CreateWishListItemDto
{
    public string ItemName { get; set; } = string.Empty;
    public int UserId { get; set; }
    public string? Description { get; set; }
    public int Quantity { get; set; } = 1;
    public decimal? EstimatedCost { get; set; }
    public string? ProductUrl { get; set; }
}

public class UpdateWishListItemDto
{
    public string? ItemName { get; set; }
    public string? Description { get; set; }
    public int? Quantity { get; set; }
    public decimal? EstimatedCost { get; set; }
    public string? ProductUrl { get; set; }
} 