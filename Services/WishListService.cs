using Microsoft.EntityFrameworkCore;
using ChristmasGiftApi.Data;
using ChristmasGiftApi.DTOs;
using ChristmasGiftApi.Interfaces;
using ChristmasGiftApi.Models;

namespace ChristmasGiftApi.Services;

public class WishListService : IBaseService<WishListItemDto, CreateWishListItemDto, UpdateWishListItemDto>
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<WishListService> _logger;

    public WishListService(ApplicationDbContext context, ILogger<WishListService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<WishListItemDto>> GetAllAsync()
    {
        try
        {
            var items = await _context.WishListItems
                .Include(w => w.User)
                .ToListAsync();
            return items.Select(MapToDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting all wish list items");
            throw;
        }
    }

    public async Task<WishListItemDto?> GetByIdAsync(int id)
    {
        try
        {
            var item = await _context.WishListItems
                .Include(w => w.User)
                .FirstOrDefaultAsync(w => w.ItemId == id);
            return item != null ? MapToDto(item) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting wish list item with ID: {ItemId}", id);
            throw;
        }
    }

    public async Task<IEnumerable<WishListItemDto>> GetByUserIdAsync(int userId)
    {
        try
        {
            var items = await _context.WishListItems
                .Include(w => w.User)
                .Where(w => w.UserId == userId)
                .ToListAsync();
            return items.Select(MapToDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting wish list items for user ID: {UserId}", userId);
            throw;
        }
    }

    public async Task<WishListItemDto> CreateAsync(CreateWishListItemDto createDto)
    {
        try
        {
            var item = new WishListItem
            {
                UserId = createDto.UserId,
                ItemName = createDto.ItemName,
                Description = createDto.Description,
                Quantity = createDto.Quantity,
                EstimatedCost = createDto.EstimatedCost,
                ProductUrl = createDto.ProductUrl,
                DateAdded = DateTime.UtcNow,
                LastModified = DateTime.UtcNow
            };

            _context.WishListItems.Add(item);
            await _context.SaveChangesAsync();

            return MapToDto(item);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating wish list item");
            throw;
        }
    }

    public async Task<WishListItemDto?> UpdateAsync(int id, UpdateWishListItemDto updateDto)
    {
        try
        {
            var item = await _context.WishListItems.FindAsync(id);
            if (item == null) return null;

            if (updateDto.ItemName != null)
                item.ItemName = updateDto.ItemName;
            if (updateDto.Description != null)
                item.Description = updateDto.Description;
            if (updateDto.Quantity.HasValue)
                item.Quantity = updateDto.Quantity.Value;
            if (updateDto.EstimatedCost.HasValue)
                item.EstimatedCost = updateDto.EstimatedCost.Value;
            if (updateDto.ProductUrl != null)
                item.ProductUrl = updateDto.ProductUrl;

            item.LastModified = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return MapToDto(item);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating wish list item with ID: {ItemId}", id);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            var item = await _context.WishListItems.FindAsync(id);
            if (item == null) return false;

            _context.WishListItems.Remove(item);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting wish list item with ID: {ItemId}", id);
            throw;
        }
    }

    private static WishListItemDto MapToDto(WishListItem item)
    {
        return new WishListItemDto
        {
            ItemId = item.ItemId,
            UserId = item.UserId,
            ItemName = item.ItemName,
            Description = item.Description,
            Quantity = item.Quantity,
            EstimatedCost = item.EstimatedCost,
            ProductUrl = item.ProductUrl,
            DateAdded = item.DateAdded,
            LastModified = item.LastModified
        };
    }
} 