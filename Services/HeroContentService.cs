using Microsoft.EntityFrameworkCore;
using ChristmasGiftApi.Data;
using ChristmasGiftApi.DTOs;
using ChristmasGiftApi.Interfaces;
using ChristmasGiftApi.Models;

namespace ChristmasGiftApi.Services;

public class HeroContentService : IBaseService<HeroContentDto, CreateHeroContentDto, UpdateHeroContentDto>
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<HeroContentService> _logger;

    public HeroContentService(ApplicationDbContext context, ILogger<HeroContentService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<HeroContentDto>> GetAllAsync()
    {
        try
        {
            var contents = await _context.HeroContent.ToListAsync();
            return contents.Select(MapToDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting all hero content");
            throw;
        }
    }

    public async Task<HeroContentDto?> GetByIdAsync(int id)
    {
        try
        {
            var content = await _context.HeroContent.FindAsync(id);
            return content != null ? MapToDto(content) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting hero content with ID: {ContentId}", id);
            throw;
        }
    }

    public async Task<HeroContentDto?> GetActiveContentAsync()
    {
        try
        {
            var content = await _context.HeroContent
                .FirstOrDefaultAsync(h => h.IsActive);
            return content != null ? MapToDto(content) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting active hero content");
            throw;
        }
    }

    public async Task<HeroContentDto> CreateAsync(CreateHeroContentDto createDto)
    {
        try
        {
            if (createDto.IsActive)
            {
                // Deactivate all other content if this one is active
                await DeactivateAllContentAsync();
            }

            var content = new HeroContent
            {
                Title = createDto.Title,
                Description = createDto.Description,
                AnimationData = createDto.AnimationData,
                IsActive = createDto.IsActive,
                CreatedDate = DateTime.UtcNow,
                LastModifiedDate = DateTime.UtcNow
            };

            _context.HeroContent.Add(content);
            await _context.SaveChangesAsync();

            return MapToDto(content);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating hero content");
            throw;
        }
    }

    public async Task<HeroContentDto?> UpdateAsync(int id, UpdateHeroContentDto updateDto)
    {
        try
        {
            var content = await _context.HeroContent.FindAsync(id);
            if (content == null) return null;

            if (updateDto.Title != null)
                content.Title = updateDto.Title;
            if (updateDto.Description != null)
                content.Description = updateDto.Description;
            if (updateDto.AnimationData != null)
                content.AnimationData = updateDto.AnimationData;
            if (updateDto.IsActive.HasValue)
            {
                if (updateDto.IsActive.Value)
                {
                    // Deactivate all other content if this one is being activated
                    await DeactivateAllContentAsync();
                }
                content.IsActive = updateDto.IsActive.Value;
            }

            content.LastModifiedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return MapToDto(content);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating hero content with ID: {ContentId}", id);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            var content = await _context.HeroContent.FindAsync(id);
            if (content == null) return false;

            _context.HeroContent.Remove(content);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting hero content with ID: {ContentId}", id);
            throw;
        }
    }

    private async Task DeactivateAllContentAsync()
    {
        var activeContent = await _context.HeroContent
            .Where(h => h.IsActive)
            .ToListAsync();

        foreach (var content in activeContent)
        {
            content.IsActive = false;
            content.LastModifiedDate = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
    }

    private static HeroContentDto MapToDto(HeroContent content)
    {
        return new HeroContentDto
        {
            ContentId = content.ContentId,
            Title = content.Title,
            Description = content.Description,
            AnimationData = content.AnimationData,
            IsActive = content.IsActive,
            CreatedDate = content.CreatedDate,
            LastModifiedDate = content.LastModifiedDate
        };
    }
} 