using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ChristmasGiftApi.DTOs;
using ChristmasGiftApi.Interfaces;
using ChristmasGiftApi.Services;
using Microsoft.Extensions.Logging;

namespace ChristmasGiftApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class RecommendWishListController : ControllerBase
    {
        private readonly IBaseService<RecommendWishListItemDto, CreateRecommendWishListItemDto, UpdateRecommendWishListItemDto> _recommendWishListService;
        private readonly ILogger<RecommendWishListController> _logger;

        public RecommendWishListController(IBaseService<RecommendWishListItemDto, CreateRecommendWishListItemDto, UpdateRecommendWishListItemDto> recommendWishListService, ILogger<RecommendWishListController> logger)
        {
            _recommendWishListService = recommendWishListService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RecommendWishListItemDto>>> GetRecommendWishListItems()
        {
            var items = await _recommendWishListService.GetAllAsync();
            return Ok(items);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RecommendWishListItemDto>> GetRecommendWishListItem(int id)
        {
            var item = await _recommendWishListService.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            return Ok(item);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<RecommendWishListItemDto>>> GetUserRecommendWishList(int userId)
        {
            try
            {
                var service = _recommendWishListService as RecommendWishListService;
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
        public async Task<ActionResult<RecommendWishListItemDto>> CreateRecommendWishListItem(CreateRecommendWishListItemDto createDto)
        {
            var item = await _recommendWishListService.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetRecommendWishListItem), new { id = item.RecommendItemId }, item);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<RecommendWishListItemDto>> UpdateRecommendWishListItem(int id, UpdateRecommendWishListItemDto updateDto)
        {
            var item = await _recommendWishListService.UpdateAsync(id, updateDto);
            if (item == null)
            {
                return NotFound();
            }

            return Ok(item);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRecommendWishListItem(int id)
        {
            var success = await _recommendWishListService.DeleteAsync(id);
            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
