using Microsoft.AspNetCore.Mvc;
using ChristmasGiftApi.DTOs;
using ChristmasGiftApi.Interfaces;
using ChristmasGiftApi.Services;

namespace ChristmasGiftApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WishListSubmissionController : ControllerBase
{
    private readonly IBaseService<WishListSubmissionDto, CreateWishListSubmissionDto, UpdateWishListSubmissionDto> _submissionService;
    private readonly ILogger<WishListSubmissionController> _logger;
    private readonly WishListNotificationService _notificationService;

    public WishListSubmissionController(
        IBaseService<WishListSubmissionDto, CreateWishListSubmissionDto, UpdateWishListSubmissionDto> submissionService, 
        ILogger<WishListSubmissionController> logger,
        WishListNotificationService notificationService)
    {
        _submissionService = submissionService;
        _logger = logger;
        _notificationService = notificationService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<WishListSubmissionDto>>> GetAllSubmissions()
    {
        try
        {
            var submissions = await _submissionService.GetAllAsync();
            return Ok(submissions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting all wish list submissions");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<WishListSubmissionDto>> GetSubmission(int id)
    {
        try
        {
            var submission = await _submissionService.GetByIdAsync(id);
            if (submission == null)
            {
                return NotFound();
            }
            return Ok(submission);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting wish list submission with ID: {SubmissionId}", id);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<WishListSubmissionDto>>> GetUserSubmissions(int userId)
    {
        try
        {
            var service = _submissionService as WishListSubmissionService;
            if (service == null)
            {
                throw new InvalidOperationException("Service not properly configured");
            }

            var submissions = await service.GetByUserIdAsync(userId);
            return Ok(submissions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting wish list submissions for user ID: {UserId}", userId);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpPost]
    public async Task<ActionResult<WishListSubmissionDto>> CreateSubmission(CreateWishListSubmissionDto createDto)
    {
        try
        {
            var submission = await _submissionService.CreateAsync(createDto);
            await _notificationService.SendWishListApprovalNotificationsAsync(
                   submission.UserId);
            return CreatedAtAction(nameof(GetSubmission), new { id = submission.SubmissionId }, submission);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating wish list submission");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSubmission(int id, UpdateWishListSubmissionDto updateDto)
    {
        try
        {
            WishListSubmissionDto? submission = await _submissionService.UpdateAsync(id, updateDto);
            if (submission == null)
            {
                return NotFound();
            }

            // Check if the status is "Waiting for Approval" (StatusId = 1)j
            if (updateDto.StatusId == 1)
            {
                await _notificationService.SendWishListApprovalNotificationsAsync(
                    submission.UserId);
            }

            return Ok(submission);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating wish list submission with ID: {SubmissionId}", id);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSubmission(int id)
    {
        try
        {
            var result = await _submissionService.DeleteAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting wish list submission with ID: {SubmissionId}", id);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }
} 