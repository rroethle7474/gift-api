using Azure.Identity;
using ChristmasGiftApi.DTOs;
using ChristmasGiftApi.Interfaces;
using ChristmasGiftApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph;

namespace ChristmasGiftApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IBaseService<UserDto, CreateUserDto, UpdateUserDto> _userService;
    private readonly IBaseService<WishListSubmissionDto, CreateWishListSubmissionDto, UpdateWishListSubmissionDto> _submissionService;
    private readonly IAuthService _authService;
    private readonly ILogger<UsersController> _logger;
    private readonly IConfiguration _configuration;

    public UsersController(
        IBaseService<UserDto, CreateUserDto, UpdateUserDto> userService, 
        ILogger<UsersController> logger,
        IConfiguration configuration,
        IAuthService authService,
        IBaseService<WishListSubmissionDto, CreateWishListSubmissionDto, UpdateWishListSubmissionDto> submissionService)
    {
        _userService = userService;
        _logger = logger;
        _configuration = configuration;
        _authService = authService;
        _submissionService = submissionService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
    {
        try
        {
            var users = await _userService.GetAllAsync();
            return Ok(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting all users");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetUser(int id)
    {
        try
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting user with ID: {UserId}", id);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpPost]
    public async Task<ActionResult<UserDto>> CreateUser(CreateUserDto createDto)
    {
        try
        {
            var user = await _userService.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetUser), new { id = user.UserId }, user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating user");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, UpdateUserDto updateDto)
    {
        try
        {
            var user = await _userService.UpdateAsync(id, updateDto);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating user with ID: {UserId}", id);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        try
        {
            var result = await _userService.DeleteAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting user with ID: {UserId}", id);
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login(LoginDto loginDto)
    {
        try
        {
            var response = await _authService.LoginAsync(loginDto);
            if (response == null)
            {
                return Unauthorized("Invalid username or password");
            }
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpPost("logout")]
    public async Task<ActionResult<bool>> Logout(LogoutDto logoutDto)
    {
        try
        {
            if(!logoutDto.IsGuestUser)
            {
                return Ok(true);
            }

            if (_userService is UserService userService && _submissionService is WishListSubmissionService submissionService)
            {
                var logoutResponse = await userService.Logout(logoutDto);
                if (logoutResponse == null)
                {
                    return StatusCode(500, "An error occurred during logout. Please contact admin.");
                }

                var wishListSubmission = await submissionService.GetByUserIdAsync(logoutResponse.UserId);
                foreach(var sub in wishListSubmission)
                {
                    await submissionService.DeleteAsync(sub.SubmissionId);
                }

                return Ok(true);
            }
            return StatusCode(500, "Service not available.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout");
            return StatusCode(500, "An error occured during logout. Please contact admin.");
        }
    }

    [HttpPost("{userId}/change-password")]
    public async Task<ActionResult<LoginResponseDto>> ChangePassword(int userId, ChangePasswordDto changePasswordDto)
    {
        try
        {
            var response = await _authService.ChangePasswordAsync(userId, changePasswordDto);

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error trying to change password");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpGet("azure-b2c")]
    public async Task<ActionResult<IEnumerable<AzureB2CUserDto>>> GetAzureB2CUsers()
    {
        try
        {
            // Create Graph client
            var scopes = new[] { "https://graph.microsoft.com/.default" };
            var clientSecretCredential = new ClientSecretCredential(
                _configuration["AzureAdB2C:TenantId"],
                _configuration["AzureAdB2C:ClientId"],
                _configuration["AzureAdB2C:ClientSecret"]);

            var graphClient = new GraphServiceClient(clientSecretCredential, scopes);

            // Get users from Azure AD B2C
            var azureUsers = await graphClient.Users
                .GetAsync(config => {
                    config.QueryParameters.Select = new[] { "id", "displayName", "userPrincipalName", "mail" };
                });

            if (azureUsers?.Value == null)
            {
                return new List<AzureB2CUserDto>();
            }

            // Get all users from your database
            var databaseUsers = await _userService.GetAllAsync();
            var databaseUserEmails = databaseUsers.Select(u => u.Email.ToLower()).ToHashSet();

            // Map and check if users exist in your database
            var userDtos = azureUsers.Value.Select(user => new AzureB2CUserDto
            {
                Id = user.Id ?? "",
                DisplayName = user.DisplayName ?? string.Empty,
                UserPrincipalName = user.UserPrincipalName ?? string.Empty,
                Email = user.Mail ?? user.UserPrincipalName ?? string.Empty,
                IsRegisteredInDatabase = databaseUserEmails.Contains(
                    (user.Mail ?? user.UserPrincipalName ?? string.Empty).ToLower())
            });

            return Ok(userDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting Azure B2C users");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpPost("azure-b2c/{objectId}/sync")]
    public async Task<ActionResult<UserDto>> SyncAzureB2CUser(string objectId)
    {
        try
        {
            // Create Graph client
            var scopes = new[] { "https://graph.microsoft.com/.default" };
            var clientSecretCredential = new ClientSecretCredential(
                _configuration["AzureAdB2C:TenantId"],
                _configuration["AzureAdB2C:ClientId"],
                _configuration["AzureAdB2C:ClientSecret"]);

            var graphClient = new GraphServiceClient(clientSecretCredential, scopes);

            // Get user from Azure AD B2C
            var azureUser = await graphClient.Users[objectId].GetAsync();

            if (azureUser == null)
            {
                return NotFound("User not found in Azure AD B2C");
            }

            // Check if user already exists in database
            var existingUsers = await _userService.GetAllAsync();
            var existingUser = existingUsers.FirstOrDefault(u =>
                u.Email.Equals(azureUser.Mail ?? azureUser.UserPrincipalName,
                StringComparison.OrdinalIgnoreCase));

            if (existingUser != null)
            {
                return Ok(existingUser);
            }

            // Create user in database
            var createDto = new CreateUserDto
            {
                Username = azureUser.DisplayName ?? azureUser.UserPrincipalName?.Split('@')[0] ?? "Unknown",
                Email = azureUser.Mail ?? azureUser.UserPrincipalName ?? string.Empty,
                IsAdmin = false, // Set default values
                IsGuestUser = false,
                SpendingLimit = null,
                SillyDescription = null,
                GreetingMessage = null
            };

            var newUser = await _userService.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetUser), new { id = newUser.UserId }, newUser);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while syncing Azure B2C user");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }
} 