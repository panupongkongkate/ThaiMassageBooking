namespace MassageBookingApi.Models
{
    public class LineTokenResponse
    {
        public string access_token { get; set; } = "";
        public string token_type { get; set; } = "";
        public string refresh_token { get; set; } = "";
        public int expires_in { get; set; }
        public string scope { get; set; } = "";
    }

    public class LineUserProfile
    {
        public string userId { get; set; } = "";
        public string displayName { get; set; } = "";
        public string pictureUrl { get; set; } = "";
        public string statusMessage { get; set; } = "";
    }

    public class LineTokenRequest
    {
        public string Code { get; set; } = "";
        public string State { get; set; } = "";
    }

    public class SendMessageRequest
    {
        public string UserId { get; set; } = "";
        public string Message { get; set; } = "";
    }

    public class OrderItem
    {
        public string Name { get; set; } = "";
        public int Quantity { get; set; }
        public double Price { get; set; }
    }

    public class SendOrderRequest
    {
        public string UserId { get; set; } = "";
        public List<OrderItem> Items { get; set; } = new();
        public double Total { get; set; }
    }

    public class LineMessage
    {
        public string type { get; set; } = "text";
        public string text { get; set; } = "";
    }

    public class LinePushMessageRequest
    {
        public string to { get; set; } = "";
        public List<LineMessage> messages { get; set; } = new();
    }

    public class SystemLineApiUsage
    {
        public int Id { get; set; }
        public int TotalMessagesSent { get; set; } = 0;
        public int MaxMessagesLimit { get; set; } = 50; // จำกัดทั้งระบบ 50 ครั้ง
        public DateTime LastResetDate { get; set; } = DateTime.UtcNow;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

}