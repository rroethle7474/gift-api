using ChristmasGiftApi.DTOs;

namespace ChristmasGiftApi.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto?> LoginAsync(LoginDto loginDto);
    Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto changePasswordDto);
} 