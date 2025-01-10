using System.ComponentModel.DataAnnotations;

namespace ChristmasGiftApi.Models
{
    public class WishListSubmissionStatus
    {
        [Key]
        public int StatusId { get; set; }

        [Required]
        [MaxLength(50)]
        public string StatusName { get; set; }

        [MaxLength(255)]
        public string? StatusDescription { get; set; }

        [Required]
        public int DisplayOrder { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime LastModifiedDate { get; set; } = DateTime.UtcNow;
    }
}