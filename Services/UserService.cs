using Microsoft.EntityFrameworkCore;
using ChristmasGiftApi.Data;
using ChristmasGiftApi.DTOs;
using ChristmasGiftApi.Interfaces;
using ChristmasGiftApi.Models;

namespace ChristmasGiftApi.Services;

public class UserService : IBaseService<UserDto, CreateUserDto, UpdateUserDto>
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<UserService> _logger;

    public UserService(ApplicationDbContext context, ILogger<UserService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<UserDto>> GetAllAsync()
    {
        try
        {
            var users = await _context.Users.ToListAsync();
            return users.Select(MapToDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting all users");
            throw;
        }
    }

    public async Task<UserDto?> GetByIdAsync(int id)
    {
        try
        {
            var user = await _context.Users.FindAsync(id);
            return user != null ? MapToDto(user) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting user with ID: {UserId}", id);
            throw;
        }
    }

    public async Task<UserDto> CreateAsync(CreateUserDto createDto)
    {
        try
        {
            var user = new User
            {
                Username = createDto.Username,
                Email = createDto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(createDto.Password),
                OriginalPassword = createDto.Password,
                IsAdmin = createDto.IsAdmin,
                IsGuestUser = createDto.IsGuestUser,
                Name = createDto.Name,
                SpendingLimit = createDto.SpendingLimit,
                SillyDescription = createDto.SillyDescription,
                GreetingMessage = createDto.GreetingMessage,
                ParentEmail1 = createDto.ParentEmail1,
                ParentEmail2 = createDto.ParentEmail2,
                ParentPhone1 = createDto.ParentPhone1,
                ParentPhone2 = createDto.ParentPhone2,
                CreatedDate = DateTime.UtcNow,
                LastModifiedDate = DateTime.UtcNow,
                Birthday = createDto.Birthday.HasValue ? createDto.Birthday.Value : null
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return MapToDto(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating user");
            throw;
        }
    }

    public async Task<UserDto?> UpdateAsync(int id, UpdateUserDto updateDto)
    {
        try
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return null;

            if (updateDto.Username != null)
                user.Username = updateDto.Username;
            if (updateDto.Email != null)
                user.Email = updateDto.Email;
            if (updateDto.Name != null) // Allow empty string
                user.Name = updateDto.Name;
            if (updateDto.IsAdmin.HasValue)
                user.IsAdmin = updateDto.IsAdmin.Value;
            if (updateDto.IsGuestUser.HasValue)
                user.IsGuestUser = updateDto.IsGuestUser.Value;
            if (updateDto.SpendingLimit.HasValue)
                user.SpendingLimit = updateDto.SpendingLimit;
            if (updateDto.SillyDescription != null) // Allow empty string
                user.SillyDescription = updateDto.SillyDescription;
            if (updateDto.GreetingMessage != null) // Allow empty string
                user.GreetingMessage = updateDto.GreetingMessage;

            // Allow empty strings to clear the fields
            user.ParentEmail1 = updateDto.ParentEmail1;
            user.ParentEmail2 = updateDto.ParentEmail2;
            user.ParentPhone1 = updateDto.ParentPhone1;
            user.ParentPhone2 = updateDto.ParentPhone2;

            if (updateDto.Birthday.HasValue)
                user.Birthday = updateDto.Birthday.Value;

            user.LastModifiedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return MapToDto(user);
        }
        catch (Exception ex)
        {
            // Handle exceptions (e.g., log the error)
            return null;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting user with ID: {UserId}", id);
            throw;
        }
    }

    public async Task<UserDto?> Logout(LogoutDto logoutDto)
    {
        try
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == logoutDto.Username);
            if (user == null) return null;

            if (user.IsGuestUser)
            {
                user.LastModifiedDate = DateTime.UtcNow;
                user.SpendingLimit = 100;
                await _context.SaveChangesAsync();
            }

            return MapToDto(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout for user {Username}", logoutDto.Username);
            throw;
        }
    }

    private static UserDto MapToDto(User user)
    {
        return new UserDto
        {
            UserId = user.UserId,
            Username = user.Username,
            Email = user.Email,
            Name = user.Name,
            IsAdmin = user.IsAdmin,
            IsGuestUser = user.IsGuestUser,
            SpendingLimit = user.SpendingLimit,
            SillyDescription = user.SillyDescription,
            GreetingMessage = user.GreetingMessage,
            ParentEmail1 = user.ParentEmail1,
            ParentEmail2 = user.ParentEmail2,
            ParentPhone1 = user.ParentPhone1,
            ParentPhone2 = user.ParentPhone2,
            Birthday = user.Birthday,
        };
    }
} 