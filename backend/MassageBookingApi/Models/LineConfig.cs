namespace MassageBookingApi.Models
{
    public class LineConfig
    {
        public string ClientId { get; set; } = "";
        public string ClientSecret { get; set; } = "";
        public string ChannelAccessToken { get; set; } = "";
        public string RedirectUrl { get; set; } = "";
        public bool EnableNotifications { get; set; } = true;
    }
}