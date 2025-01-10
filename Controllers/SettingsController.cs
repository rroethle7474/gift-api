// Controllers/SettingsController.cs
using Microsoft.AspNetCore.Mvc;
using ChristmasGiftApi.DTOs;
using ChristmasGiftApi.Interfaces;

namespace ChristmasGiftApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SettingsController : ControllerBase
{
    private readonly ISettingsService _settingsService;
    private readonly ILogger<SettingsController> _logger;

    public SettingsController(
        ISettingsService settingsService,
        ILogger<SettingsController> logger)
    {
        _settingsService = settingsService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SettingDto>>> GetAllSettings()
    {
        try
        {
            var settings = await _settingsService.GetAllSettingsAsync();
            return Ok(settings);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting all settings");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpGet("{name}")]
    public async Task<ActionResult<SettingDto>> GetSettingByName(string name)
    {
        try
        {
            var setting = await _settingsService.GetSettingByNameAsync(name);
            if (setting == null)
            {
                return NotFound();
            }
            return Ok(setting);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting setting with name: {SettingName}", name);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }
}