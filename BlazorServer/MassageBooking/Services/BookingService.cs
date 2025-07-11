using MassageBooking.Models;
using System.Text;
using System.Text.Json;

namespace MassageBooking.Services
{
    public class BookingService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly string _baseUrl;

        public BookingService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _baseUrl = _configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5000/api";
        }

        public async Task<List<Booking>> GetBookingsByMonthAsync(int year, int month)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/booking/month/{year}/{month}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var bookings = JsonSerializer.Deserialize<List<Booking>>(json, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    });
                    return bookings ?? new List<Booking>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting bookings: {ex.Message}");
            }
            return new List<Booking>();
        }

        public async Task<BookingResponse?> AddBookingAsync(BookingRequest booking)
        {
            try
            {
                var json = JsonSerializer.Serialize(booking, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{_baseUrl}/booking", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    var bookingData = JsonSerializer.Deserialize<BookingResponse>(responseJson, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    });
                    if (bookingData != null)
                    {
                        bookingData.Success = true;
                    }
                    return bookingData;
                }
                else
                {
                    var errorJson = await response.Content.ReadAsStringAsync();
                    try
                    {
                        var errorData = JsonSerializer.Deserialize<JsonElement>(errorJson);
                        var message = "เกิดข้อผิดพลาด";
                        
                        if (errorData.TryGetProperty("message", out var msgProperty))
                        {
                            message = msgProperty.GetString() ?? message;
                        }
                        
                        return new BookingResponse { Success = false, Message = message };
                    }
                    catch
                    {
                        return new BookingResponse { Success = false, Message = "เกิดข้อผิดพลาด" };
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding booking: {ex.Message}");
                return new BookingResponse { Success = false, Message = $"เกิดข้อผิดพลาด: {ex.Message}" };
            }
        }

        public async Task<List<Booking>> GetAllBookingsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/booking");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var bookings = JsonSerializer.Deserialize<List<Booking>>(json, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    });
                    return bookings ?? new List<Booking>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting all bookings: {ex.Message}");
            }
            return new List<Booking>();
        }
    }
}