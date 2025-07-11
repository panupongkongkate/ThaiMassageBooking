using MassageBooking.Models;
using System.Text.Json;

namespace MassageBooking.Services
{
    public class MasseurService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly string _baseUrl;

        public MasseurService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _baseUrl = _configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5000/api";
        }

        public async Task<List<Masseur>> GetAllMasseursAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/masseur");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var masseurs = JsonSerializer.Deserialize<List<Masseur>>(json, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    });
                    return masseurs ?? new List<Masseur>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting masseurs: {ex.Message}");
            }
            return new List<Masseur>();
        }

        public async Task<List<string>> GetMasseurNamesAsync()
        {
            try
            {
                var masseurs = await GetAllMasseursAsync();
                return masseurs.Where(m => m.IsActive).Select(m => m.Name).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting masseur names: {ex.Message}");
                return new List<string>();
            }
        }

        public async Task<Masseur?> GetMasseurByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/masseur/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<Masseur>(json, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting masseur by id: {ex.Message}");
            }
            return null;
        }
    }
}