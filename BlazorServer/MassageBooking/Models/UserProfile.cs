namespace MassageBooking.Models
{
    public class UserProfile
    {
        public string UserId { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string? PictureUrl { get; set; }
        
        // Helper properties
        public bool HasPicture => !string.IsNullOrEmpty(PictureUrl);
        public string InitialLetter => string.IsNullOrEmpty(DisplayName) ? "?" : DisplayName[0].ToString().ToUpper();
    }

    public class LineTokenRequest
    {
        public string Code { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
    }

    public class LineConfig
    {
        public string ClientId { get; set; } = string.Empty;
        public string RedirectUrl { get; set; } = string.Empty;
        public bool EnableNotifications { get; set; } = true;
    }

    public class LineTokenResponse
    {
        public bool Success { get; set; }
        public UserProfile? User { get; set; }
        public string? AccessToken { get; set; }
    }
}