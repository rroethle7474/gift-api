using Microsoft.EntityFrameworkCore;
using ChristmasGiftApi.Data;
using ChristmasGiftApi.DTOs;
using ChristmasGiftApi.Interfaces;
using ChristmasGiftApi.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Graph.Models.Security;

namespace ChristmasGiftApi.Services;

public class WishListSubmissionService : IBaseService<WishListSubmissionDto, CreateWishListSubmissionDto, UpdateWishListSubmissionDto>
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<WishListSubmissionService> _logger;
    private readonly IMemoryCache _cache;
    private const string CacheKey = "WishListSubmissionStatuses";

    public WishListSubmissionService(ApplicationDbContext context, ILogger<WishListSubmissionService> logger, IMemoryCache cache)
    {
        _context = context;
        _logger = logger;
        _cache = cache;
    }

    public async Task<IEnumerable<WishListSubmissionDto>> GetAllAsync()
    {
        try
        {
            var submissions = await _context.WishListSubmissions
                .Include(w => w.User)
                .Include(s => s.Status)
                .Where(wl => wl.IsActive)
                .ToListAsync();
            return submissions.Select(MapToDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting all wish list submissions");
            throw;
        }
    }

    public async Task<WishListSubmissionDto?> GetByIdAsync(int id)
    {
        try
        {
            var submission = await _context.WishListSubmissions
                .Include(w => w.User)
                .Include(s => s.Status)
                .FirstOrDefaultAsync(w => w.SubmissionId == id);
            return submission != null ? MapToDto(submission) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting wish list submission with ID: {SubmissionId}", id);
            throw;
        }
    }

    public async Task<IEnumerable<WishListSubmissionDto>> GetByUserIdAsync(int userId)
    {
        try
        {
            var submissions = await _context.WishListSubmissions
                .Include(w => w.User)
                .Include(s => s.Status)
                .Where(w => w.UserId == userId)
                .ToListAsync();
            return submissions.Select(MapToDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting wish list submissions for user ID: {UserId}", userId);
            throw;
        }
    }

    public async Task<WishListSubmissionDto> CreateAsync(CreateWishListSubmissionDto createDto)
    {
        var wishListSubmissionStatuses = await GetWishListSubmissionStatusesAsync();
        if(wishListSubmissionStatuses == null || wishListSubmissionStatuses.Count == 0)
        {
            throw new InvalidOperationException("Wish list submission statuses not found");
        }

        var wishListSubmissionStatus = wishListSubmissionStatuses.OrderBy(s=> s.DisplayOrder).FirstOrDefault();

        if(wishListSubmissionStatus == null)
        {
            throw new InvalidOperationException("Wish list submission status not found");
        }

        var submission = new WishListSubmission
        {
            UserId = createDto.UserId,
            StatusId = wishListSubmissionStatus.StatusId,
            SubmissionDate = DateTime.UtcNow,
            LastModified = DateTime.UtcNow,
            Reason = string.Empty,
        };

        _context.WishListSubmissions.Add(submission);
        await _context.SaveChangesAsync();

        return MapToDto(submission);
    }

    public async Task<WishListSubmissionDto?> UpdateAsync(int id, UpdateWishListSubmissionDto updateDto)
    {
        var submission = await _context.WishListSubmissions.FindAsync(id);
        if (submission == null) return null;

        if (updateDto?.ShipmentDate.HasValue ?? false)
            submission.ShipmentDate = updateDto.ShipmentDate;

        if (updateDto.MakeInactive)
        {
            submission.IsActive = false;
            submission.LastModified = DateTime.UtcNow;
            submission.Reason = updateDto.Reason;
            if(updateDto.StatusId != 0)
            {
                submission.StatusId = updateDto.StatusId;
            }
        }
        else
        {
            submission.StatusId = updateDto.StatusId;
            submission.LastModified = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
        return MapToDto(submission);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            var submission = await _context.WishListSubmissions.FindAsync(id);
            if (submission == null) return false;

            _context.WishListSubmissions.Remove(submission);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting wish list submission with ID: {SubmissionId}", id);
            throw;
        }
    }

    private async Task<List<WishListSubmissionStatus>> GetWishListSubmissionStatusesAsync()
    {
        try
        {
            if (!_cache.TryGetValue(CacheKey, out List<WishListSubmissionStatus>? statuses))
            {
                statuses = await _context.WishListSubmissionStatus.ToListAsync();
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromHours(24));
                _cache.Set(CacheKey, statuses, cacheEntryOptions);
            }

            return statuses ?? new List<WishListSubmissionStatus>();
        }
        catch(Exception e)
        {
            string message = e.ToString();
            throw new Exception("BAD");
        }
    }

    private static WishListSubmissionDto MapToDto(WishListSubmission submission)
    {
        return new WishListSubmissionDto
        {
            SubmissionId = submission.SubmissionId,
            UserId = submission.UserId,
            StatusId = submission.StatusId,
            IsActive = submission.IsActive,
            StatusName = submission.Status?.StatusName ?? string.Empty,
            SubmissionDate = submission.SubmissionDate,
            ShipmentDate = submission.ShipmentDate,
            LastModified = submission.LastModified,
            UserName = submission.User?.Username ?? string.Empty,
            Reason = submission.Reason ?? string.Empty
        };
    }
} 