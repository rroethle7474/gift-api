namespace ChristmasGiftApi.DTOs
{
    public class AzureB2CUserDto
    {
        public string Id { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string UserPrincipalName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsRegisteredInDatabase { get; set; }
    }
}
