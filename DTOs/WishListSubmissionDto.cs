namespace ChristmasGiftApi.DTOs
{
    public class WishListSubmissionDto
    {
        public int SubmissionId { get; set; }
        public int UserId { get; set; }
        public int StatusId { get; set; } // New property for StatusId
        public string StatusName { get; set; } = string.Empty; // New property for StatusName
        public string Reason { get; set; }
        public bool IsActive { get; set; }
        public DateTime SubmissionDate { get; set; }
        public DateTime LastModified { get; set; }
        public DateTime? ShipmentDate { get; set; }
        public string UserName { get; set; } = string.Empty;
    }

    public class CreateWishListSubmissionDto
    {
        public int UserId { get; set; }
    }

    public class UpdateWishListSubmissionDto
    {
        public int StatusId { get; set; } // New property for StatusId
        public bool MakeInactive { get; set; } = false;
        public string Reason { get; set; }
        public DateTime? ShipmentDate { get; set; }
    }
}
