using MassageBooking.Models;
using System.Text;
using System.Text.Json;

namespace MassageBooking.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly string _baseUrl;

        public ApiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _baseUrl = _configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5000/api";
        }

        // LINE API methods
        public async Task<LineConfig?> GetLineConfigAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/line/config");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<LineConfig>(json, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting LINE config: {ex.Message}");
            }
            return null;
        }

        public async Task<UserProfile?> ExchangeTokenForProfileAsync(string code, string state)
        {
            try
            {
                var request = new LineTokenRequest { Code = code, State = state };
                var json = JsonSerializer.Serialize(request, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{_baseUrl}/line/token", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    var tokenResponse = JsonSerializer.Deserialize<LineTokenResponse>(responseJson, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    });
                    
                    return tokenResponse?.Success == true ? tokenResponse.User : null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error exchanging token: {ex.Message}");
            }
            return null;
        }

        public async Task<bool> SendMessageAsync(string userId, string message)
        {
            try
            {
                // ตรวจสอบว่าเปิดใช้งานการส่งข้อความหรือไม่
                var lineConfig = await GetLineConfigAsync();
                if (lineConfig?.EnableNotifications != true)
                {
                    Console.WriteLine("[FRONTEND] LINE notifications are disabled");
                    return true; // Return true เพื่อไม่ให้เกิด error แต่ไม่ส่งจริง
                }

                var request = new { UserId = userId, Message = message };
                var json = JsonSerializer.Serialize(request, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{_baseUrl}/line/send-message", content);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending message: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> SendOrderConfirmationAsync(string userId, List<Dictionary<string, object>> items, double total)
        {
            try
            {
                var request = new { UserId = userId, Items = items, Total = total };
                var json = JsonSerializer.Serialize(request, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{_baseUrl}/line/send-order-confirmation", content);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending order confirmation: {ex.Message}");
                return false;
            }
        }
    }
}