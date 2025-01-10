using ChristmasGiftApi.DTOs;

namespace ChristmasGiftApi.Interfaces
{
    public interface ISettingsService
    {
        Task<IEnumerable<SettingDto>> GetAllSettingsAsync();
        Task<SettingDto?> GetSettingByNameAsync(string name);
    }
}
