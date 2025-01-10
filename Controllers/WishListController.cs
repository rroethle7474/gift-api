using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ChristmasGiftApi.DTOs;
using ChristmasGiftApi.Interfaces;
using ChristmasGiftApi.Services;

namespace ChristmasGiftApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class WishListController : ControllerBase
{
    private readonly IBaseService<WishListItemDto, CreateWishListItemDto, UpdateWishListItemDto> _wishListService;
    private readonly ILogger<WishListController> _logger;

    public WishListController(
        IBaseService<WishListItemDto, CreateWishListItemDto, UpdateWishListItemDto> wishListService,
        ILogger<WishListController> logger)
    {
        _wishListService = wishListService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<WishListItemDto>>> GetWishListItems()
    {
        try
        {
            var items = await _wishListService.GetAllAsync();
            return Ok(items);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting all wish list items");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<WishListItemDto>> GetWishListItem(int id)
    {
        try
        {
            var item = await _wishListService.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            return Ok(item);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting wish list item with ID: {ItemId}", id);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<WishListItemDto>>> GetUserWishList(int userId)
    {
        try
        {
            var service = _wishListService as WishListService;
            if (service == null)
            {
                throw new InvalidOperationException("Service not properly configured");
            }

            var items = await service.GetByUserIdAsync(userId);
            return Ok(items);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting wish list items for user ID: {UserId}", userId);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpPost]
    public async Task<ActionResult<WishListItemDto>> CreateWishListItem(CreateWishListItemDto createDto)
    {
        try
        {
            var item = await _wishListService.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetWishListItem), new { id = item.ItemId }, item);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating wish list item");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateWishListItem(int id, UpdateWishListItemDto updateDto)
    {
        try
        {
            var item = await _wishListService.UpdateAsync(id, updateDto);
            if (item == null)
            {
                return NotFound();
            }
            return Ok(item);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating wish list item with ID: {ItemId}", id);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteWishListItem(int id)
    {
        try
        {
            var result = await _wishListService.DeleteAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting wish list item with ID: {ItemId}", id);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }
} 