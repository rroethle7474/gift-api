namespace ChristmasGiftApi.DTOs;

public class LogoutDto
{
    public string Username { get; set; } = string.Empty;
    public bool IsGuestUser { get; set; } = false;
}