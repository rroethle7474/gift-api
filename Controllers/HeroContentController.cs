using Microsoft.AspNetCore.Mvc;
using ChristmasGiftApi.DTOs;
using ChristmasGiftApi.Interfaces;
using ChristmasGiftApi.Services;

namespace ChristmasGiftApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HeroContentController : ControllerBase
{
    private readonly IBaseService<HeroContentDto, CreateHeroContentDto, UpdateHeroContentDto> _heroContentService;
    private readonly ILogger<HeroContentController> _logger;

    public HeroContentController(
        IBaseService<HeroContentDto, CreateHeroContentDto, UpdateHeroContentDto> heroContentService,
        ILogger<HeroContentController> logger)
    {
        _heroContentService = heroContentService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<HeroContentDto>>> GetAllHeroContent()
    {
        try
        {
            var contents = await _heroContentService.GetAllAsync();
            return Ok(contents);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting all hero content");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<HeroContentDto>> GetHeroContent(int id)
    {
        try
        {
            var content = await _heroContentService.GetByIdAsync(id);
            if (content == null)
            {
                return NotFound();
            }
            return Ok(content);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting hero content with ID: {ContentId}", id);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpGet("active")]
    public async Task<ActionResult<HeroContentDto>> GetActiveHeroContent()
    {
        try
        {
            var service = _heroContentService as HeroContentService;
            if (service == null)
            {
                throw new InvalidOperationException("Service not properly configured");
            }

            var content = await service.GetActiveContentAsync();
            if (content == null)
            {
                return NotFound();
            }
            return Ok(content);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting active hero content");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpPost]
    public async Task<ActionResult<HeroContentDto>> CreateHeroContent(CreateHeroContentDto createDto)
    {
        try
        {
            var content = await _heroContentService.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetHeroContent), new { id = content.ContentId }, content);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating hero content");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateHeroContent(int id, UpdateHeroContentDto updateDto)
    {
        try
        {
            var content = await _heroContentService.UpdateAsync(id, updateDto);
            if (content == null)
            {
                return NotFound();
            }
            return Ok(content);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating hero content with ID: {ContentId}", id);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteHeroContent(int id)
    {
        try
        {
            var result = await _heroContentService.DeleteAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting hero content with ID: {ContentId}", id);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }
} 