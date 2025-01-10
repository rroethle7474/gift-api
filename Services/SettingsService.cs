// Services/SettingsService.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using ChristmasGiftApi.Data;
using ChristmasGiftApi.DTOs;
using ChristmasGiftApi.Interfaces;

namespace ChristmasGiftApi.Services;

public class SettingsService : ISettingsService
{
    private readonly ApplicationDbContext _context;
    private readonly IMemoryCache _cache;
    private readonly ILogger<SettingsService> _logger;
    private const string AllSettingsCacheKey = "AllSettings";
    private const string SettingKeyCachePrefix = "Setting_";

    public SettingsService(
        ApplicationDbContext context,
        IMemoryCache cache,
        ILogger<SettingsService> logger)
    {
        _context = context;
        _cache = cache;
        _logger = logger;
    }

    public async Task<IEnumerable<SettingDto>> GetAllSettingsAsync()
    {
        try
        {
            // Try to get from cache first
            if (_cache.TryGetValue(AllSettingsCacheKey, out IEnumerable<SettingDto>? cachedSettings))
            {
                return cachedSettings!;
            }

            // If not in cache, get from database
            var settings = await _context.Settings
                .Select(s => new SettingDto
                {
                    Name = s.Name.Trim().ToLower(),
                    Value = s.Value
                })
                .ToListAsync();

            // Cache the results
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromHours(24));

            _cache.Set(AllSettingsCacheKey, settings, cacheEntryOptions);

            return settings;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting all settings");
            throw;
        }
    }

    private string NormalizeName(string name)
    {
        return name.ToUpperInvariant();
    }

    public async Task<SettingDto?> GetSettingByNameAsync(string name)
    {
        try
        {
            string normalizedName = NormalizeName(name);
            string cacheKey = $"{SettingKeyCachePrefix}{normalizedName}";

            // Try to get from cache first
            if (_cache.TryGetValue(cacheKey, out SettingDto? cachedSetting))
            {
                return cachedSetting;
            }

            // If not in cache, get from database
            var setting = await _context.Settings
                .Where(s => s.Name == name)
                .Select(s => new SettingDto
                {
                    Name = s.Name,
                    Value = s.Value
                })
                .FirstOrDefaultAsync();

            if (setting != null)
            {
                // Cache the result
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromHours(24));

                _cache.Set(cacheKey, setting, cacheEntryOptions);
            }

            return setting;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting setting with name: {SettingName}", name);
            throw;
        }
    }
}