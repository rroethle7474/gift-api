using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChristmasGiftApi.Models
{
    public class WishListSubmission
    {
        [Key]
        public int SubmissionId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int StatusId { get; set; } // New property for StatusId

        public bool IsActive { get; set; } = true; // New property for IsActive
        public string? Reason { get; set; } = string.Empty;

        public DateTime SubmissionDate { get; set; }

        public DateTime LastModified { get; set; }
        public DateTime? ShipmentDate { get; set; }

        // Navigation properties
        [ForeignKey(nameof(UserId))]
        public User? User { get; set; }

        [ForeignKey(nameof(StatusId))]
        public WishListSubmissionStatus? Status { get; set; } // New navigation property for Status
    }
}
