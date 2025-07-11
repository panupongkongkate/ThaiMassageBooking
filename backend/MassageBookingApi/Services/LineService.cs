using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using MassageBookingApi.Models;
using MassageBookingApi.Data;

namespace MassageBookingApi.Services
{
    public class LineService
    {
        private readonly HttpClient _httpClient;
        private readonly LineConfig _lineConfig;
        private readonly BookingContext _context;

        public LineService(HttpClient httpClient, IOptions<LineConfig> lineConfig, BookingContext context)
        {
            _httpClient = httpClient;
            _lineConfig = lineConfig.Value;
            _context = context;
        }

        public async Task<LineTokenResponse?> ExchangeCodeForToken(string code, string state)
        {
            try
            {
                var tokenUrl = "https://api.line.me/oauth2/v2.1/token";
                var parameters = new Dictionary<string, string>
                {
                    ["grant_type"] = "authorization_code",
                    ["code"] = code,
                    ["redirect_uri"] = _lineConfig.RedirectUrl,
                    ["client_id"] = _lineConfig.ClientId,
                    ["client_secret"] = _lineConfig.ClientSecret
                };

                Console.WriteLine($"[LINE TOKEN DEBUG] Sending request to: {tokenUrl}");
                Console.WriteLine($"[LINE TOKEN DEBUG] Parameters:");
                Console.WriteLine($"  grant_type: {parameters["grant_type"]}");
                Console.WriteLine($"  code: {code}");
                Console.WriteLine($"  redirect_uri: {parameters["redirect_uri"]}");
                Console.WriteLine($"  client_id: {parameters["client_id"]}");
                Console.WriteLine($"  client_secret: {parameters["client_secret"]?.Substring(0, Math.Min(4, parameters["client_secret"].Length))}****");

                var content = new FormUrlEncodedContent(parameters);
                var response = await _httpClient.PostAsync(tokenUrl, content);

                Console.WriteLine($"[LINE TOKEN DEBUG] Response Status: {response.StatusCode}");
                
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"[LINE TOKEN DEBUG] Response Content: {responseContent}");

                if (response.IsSuccessStatusCode)
                {
                    return JsonSerializer.Deserialize<LineTokenResponse>(responseContent);
                }
                else
                {
                    Console.WriteLine($"[LINE TOKEN ERROR] Failed with status {response.StatusCode}: {responseContent}");
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LINE TOKEN EXCEPTION] {ex.Message}");
                Console.WriteLine($"[LINE TOKEN EXCEPTION] Stack Trace: {ex.StackTrace}");
                return null;
            }
        }

        public async Task<LineUserProfile?> GetUserProfile(string accessToken)
        {
            try
            {
                var profileUrl = "https://api.line.me/v2/profile";
                
                Console.WriteLine($"[LINE PROFILE DEBUG] Getting profile from: {profileUrl}");
                Console.WriteLine($"[LINE PROFILE DEBUG] Access Token: {accessToken?.Substring(0, Math.Min(10, accessToken.Length))}****");

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

                var response = await _httpClient.GetAsync(profileUrl);

                Console.WriteLine($"[LINE PROFILE DEBUG] Response Status: {response.StatusCode}");
                
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"[LINE PROFILE DEBUG] Response Content: {responseContent}");

                if (response.IsSuccessStatusCode)
                {
                    return JsonSerializer.Deserialize<LineUserProfile>(responseContent);
                }
                else
                {
                    Console.WriteLine($"[LINE PROFILE ERROR] Failed with status {response.StatusCode}: {responseContent}");
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LINE PROFILE EXCEPTION] {ex.Message}");
                Console.WriteLine($"[LINE PROFILE EXCEPTION] Stack Trace: {ex.StackTrace}");
                return null;
            }
        }

        public async Task<bool> SendMessage(string userId, string message)
        {
            try
            {
                // ตรวจสอบว่าเปิดใช้งานการส่งข้อความหรือไม่
                if (!_lineConfig.EnableNotifications)
                {
                    Console.WriteLine("[LINE MESSAGE] Notifications are disabled in configuration");
                    return true; // Return true เพื่อไม่ให้เกิด error แต่ไม่ส่งจริง
                }

                // ตรวจสอบจำนวนครั้งทั้งระบบ
                var canSend = await CheckSystemMessageLimit();
                if (!canSend)
                {
                    return false; // เกินจำนวนครั้งที่กำหนดทั้งระบบ (50 ครั้ง)
                }

                var messagingUrl = "https://api.line.me/v2/bot/message/push";
                
                var requestBody = new LinePushMessageRequest
                {
                    to = userId,
                    messages = new List<LineMessage>
                    {
                        new LineMessage { type = "text", text = message }
                    }
                };

                var json = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_lineConfig.ChannelAccessToken}");

                var response = await _httpClient.PostAsync(messagingUrl, content);
                
                if (response.IsSuccessStatusCode)
                {
                    // อัปเดตจำนวนครั้งที่ส่งสำเร็จทั้งระบบ
                    await IncrementSystemMessageCount();
                    return true;
                }
                
                return false;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> SendOrderConfirmation(string userId, List<OrderItem> items, double total)
        {
            var orderDetails = "🍣 คำสั่งซื้อของคุณ\n\n";
            
            foreach (var item in items)
            {
                orderDetails += $"• {item.Name} x{item.Quantity} = ฿{item.Price * item.Quantity}\n";
            }
            
            orderDetails += $"\n💰 ยอดรวม: ฿{total}";
            orderDetails += "\n\n✅ ขอบคุณที่ใช้บริการนวดไทย!";
            
            return await SendMessage(userId, orderDetails);
        }

        private async Task<bool> CheckSystemMessageLimit()
        {
            var systemUsage = await _context.SystemLineApiUsages.FirstOrDefaultAsync();
            
            if (systemUsage == null)
            {
                // สร้างใหม่หากไม่มีข้อมูลระบบ
                systemUsage = new SystemLineApiUsage
                {
                    TotalMessagesSent = 0,
                    MaxMessagesLimit = 50,
                    LastResetDate = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _context.SystemLineApiUsages.Add(systemUsage);
                await _context.SaveChangesAsync();
            }

            // ตรวจสอบว่าเกิน limit ทั้งระบบหรือไม่
            return systemUsage.TotalMessagesSent < systemUsage.MaxMessagesLimit;
        }

        private async Task IncrementSystemMessageCount()
        {
            var systemUsage = await _context.SystemLineApiUsages.FirstOrDefaultAsync();
            if (systemUsage != null)
            {
                systemUsage.TotalMessagesSent++;
                systemUsage.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<SystemLineApiUsage?> GetSystemUsageInfo()
        {
            return await _context.SystemLineApiUsages.FirstOrDefaultAsync();
        }

    }
}