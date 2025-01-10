namespace ChristmasGiftApi.DTOs;

public class UserDto
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsAdmin { get; set; }
    public bool IsParent { get; set; }
    public decimal? SpendingLimit { get; set; }
    public string? SillyDescription { get; set; }
    public string? GreetingMessage { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? ParentEmail1 { get; set; } = string.Empty;
    public string? ParentEmail2 { get; set; } = string.Empty;
    public string? ParentPhone1 { get; set; } = string.Empty;
    public string? ParentPhone2 { get; set; } = string.Empty;
    public DateTime? Birthday { get; set; }
}

public class CreateUserDto
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsAdmin { get; set; }
    public decimal? SpendingLimit { get; set; }
    public string? SillyDescription { get; set; }
    public string? GreetingMessage { get; set; }
    public string? ParentEmail1 { get; set; } = string.Empty;
    public string? ParentEmail2 { get; set; } = string.Empty;
    public string? ParentPhone1 { get; set; } = string.Empty;
    public string? ParentPhone2 { get; set; } = string.Empty;
    public DateTime? Birthday { get; set; }
}

public class UpdateUserDto
{
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? Name { get; set; }
    public bool? IsAdmin { get; set; }
    public decimal? SpendingLimit { get; set; }
    public string? SillyDescription { get; set; }
    public string? GreetingMessage { get; set; }
    public string? ParentEmail1 { get; set; } = string.Empty;
    public string? ParentEmail2 { get; set; } = string.Empty;
    public string? ParentPhone1 { get; set; } = string.Empty;
    public string? ParentPhone2 { get; set; } = string.Empty;
    public DateTime? Birthday { get; set; }
} 