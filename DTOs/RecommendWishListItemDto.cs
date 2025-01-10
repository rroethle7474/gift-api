namespace ChristmasGiftApi.DTOs
{
    public class RecommendWishListItemDto
    {
        public int RecommendItemId { get; set; }
        public int UserId { get; set; }
        public string ItemName { get; set; }
        public string? Description { get; set; }
        public string? ProductUrl { get; set; }
        public string? ProductSrcImage { get; set; }
        public decimal? EstimatedCost { get; set; }
        public int DefaultQuantity { get; set; }
        public bool IsActive { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime LastModified { get; set; }
    }

    public class CreateRecommendWishListItemDto
    {
        public int UserId { get; set; }
        public string ItemName { get; set; }
        public string? Description { get; set; }
        public string? ProductUrl { get; set; }
        public string? ProductSrcImage { get; set; }
        public decimal? EstimatedCost { get; set; }
        public int DefaultQuantity { get; set; }
        public bool IsActive { get; set; }
    }

    public class UpdateRecommendWishListItemDto
    {
        public string? ItemName { get; set; }
        public string? Description { get; set; }
        public string? ProductUrl { get; set; }
        public string? ProductSrcImage { get; set; }
        public decimal? EstimatedCost { get; set; }
        public int? DefaultQuantity { get; set; }
        public bool? IsActive { get; set; }
    }
}
