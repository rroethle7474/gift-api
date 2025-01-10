using Microsoft.EntityFrameworkCore;
using ChristmasGiftApi.Data;
using ChristmasGiftApi.DTOs;
using ChristmasGiftApi.Interfaces;
using ChristmasGiftApi.Models;
using Microsoft.Extensions.Logging;

namespace ChristmasGiftApi.Services
{
    public class RecommendWishListService : IBaseService<RecommendWishListItemDto, CreateRecommendWishListItemDto, UpdateRecommendWishListItemDto>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RecommendWishListService> _logger;

        public RecommendWishListService(ApplicationDbContext context, ILogger<RecommendWishListService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<RecommendWishListItemDto>> GetAllAsync()
        {
            var items = await _context.RecommendWishListItems.ToListAsync();
            return items.Select(MapToDto);
        }

        public async Task<RecommendWishListItemDto?> GetByIdAsync(int id)
        {
            var item = await _context.RecommendWishListItems.FindAsync(id);
            return item == null ? null : MapToDto(item);
        }

        public async Task<IEnumerable<RecommendWishListItemDto>> GetByUserIdAsync(int userId)
        {
            var items = await _context.RecommendWishListItems
                .Where(w => w.UserId == userId)
                .ToListAsync();
            return items.Select(MapToDto);
        }

        public async Task<RecommendWishListItemDto> CreateAsync(CreateRecommendWishListItemDto createDto)
        {
            var item = new RecommendWishListItem
            {
                UserId = createDto.UserId,
                ItemName = createDto.ItemName,
                Description = createDto.Description,
                ProductUrl = createDto.ProductUrl,
                ProductSrcImage = createDto.ProductSrcImage,
                EstimatedCost = createDto.EstimatedCost,
                DefaultQuantity = createDto.DefaultQuantity,
                IsActive = createDto.IsActive,
                DateAdded = DateTime.UtcNow,
                LastModified = DateTime.UtcNow
            };

            _context.RecommendWishListItems.Add(item);
            await _context.SaveChangesAsync();

            return MapToDto(item);
        }

        public async Task<RecommendWishListItemDto?> UpdateAsync(int id, UpdateRecommendWishListItemDto updateDto)
        {
            var item = await _context.RecommendWishListItems.FindAsync(id);
            if (item == null)
            {
                return null;
            }

            item.ItemName = updateDto.ItemName ?? item.ItemName;
            item.Description = updateDto.Description ?? item.Description;
            item.ProductUrl = updateDto.ProductUrl ?? item.ProductUrl;
            item.ProductSrcImage = updateDto.ProductSrcImage ?? item.ProductSrcImage;
            item.EstimatedCost = updateDto.EstimatedCost ?? item.EstimatedCost;
            item.DefaultQuantity = updateDto.DefaultQuantity ?? item.DefaultQuantity;
            item.IsActive = updateDto.IsActive ?? item.IsActive;
            item.LastModified = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return MapToDto(item);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var item = await _context.RecommendWishListItems.FindAsync(id);
            if (item == null)
            {
                return false;
            }

            _context.RecommendWishListItems.Remove(item);
            await _context.SaveChangesAsync();

            return true;
        }

        private static RecommendWishListItemDto MapToDto(RecommendWishListItem item)
        {
            return new RecommendWishListItemDto
            {
                RecommendItemId = item.RecommendItemId,
                UserId = item.UserId,
                ItemName = item.ItemName,
                Description = item.Description,
                ProductUrl = item.ProductUrl,
                ProductSrcImage = item.ProductSrcImage,
                EstimatedCost = item.EstimatedCost,
                DefaultQuantity = item.DefaultQuantity,
                IsActive = item.IsActive,
                DateAdded = item.DateAdded,
                LastModified = item.LastModified
            };
        }
    }
}
