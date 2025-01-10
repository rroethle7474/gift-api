using Microsoft.EntityFrameworkCore;
using ChristmasGiftApi.Models;

namespace ChristmasGiftApi.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<WishListItem> WishListItems { get; set; } = null!;
    public DbSet<WishListSubmission> WishListSubmissions { get; set; } = null!;
    public DbSet<HeroContent> HeroContent { get; set; } = null!;
    public DbSet<WishListSubmissionStatus> WishListSubmissionStatus { get; set; } = null!; // New DbSet for WishListSubmissionStatus
    public DbSet<RecommendWishListItem> RecommendWishListItems { get; set; } = null!; // New DbSet for RecommendWishListItem
    public DbSet<Setting> Settings { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure default values
        modelBuilder.Entity<User>()
            .Property(u => u.CreatedDate)
            .HasDefaultValueSql("GETDATE()");

        modelBuilder.Entity<User>()
            .Property(u => u.LastModifiedDate)
            .HasDefaultValueSql("GETDATE()");

        modelBuilder.Entity<WishListItem>()
            .Property(w => w.DateAdded)
            .HasDefaultValueSql("GETDATE()");

        modelBuilder.Entity<WishListItem>()
            .Property(w => w.LastModified)
            .HasDefaultValueSql("GETDATE()");

        modelBuilder.Entity<WishListSubmission>()
            .Property(w => w.SubmissionDate)
            .HasDefaultValueSql("GETDATE()");

        modelBuilder.Entity<WishListSubmission>()
            .Property(w => w.LastModified)
            .HasDefaultValueSql("GETDATE()");

        modelBuilder.Entity<HeroContent>()
            .Property(h => h.CreatedDate)
            .HasDefaultValueSql("GETDATE()");

        modelBuilder.Entity<HeroContent>()
            .Property(h => h.LastModifiedDate)
            .HasDefaultValueSql("GETDATE()");
    }
} 