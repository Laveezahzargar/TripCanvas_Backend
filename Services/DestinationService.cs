using P6_Travel_Planner_Backend.DTOs;
using System.Text.Json;

namespace P6_Travel_Planner_Backend.Services
{
    public class DestinationService
    {
        private readonly HttpClient _http;
        private readonly string _apiKey;

        public DestinationService(HttpClient http, IConfiguration configuration)
        {
            _http = http;
            _apiKey = configuration["Geoapify:ApiKey"] ?? throw new InvalidOperationException("Geoapify API key not configured."); 
        }

        public async Task<PlacesResponse?> GetPlaces(double lat, double lon)
        {
            try
            { 
                var url = $"https://api.geoapify.com/v2/places" +
                $"?categories=tourism.sights" +
                $"&filter=circle:{lon},{lat},5000" +
                $"&limit=20" +
                $"&apiKey={_apiKey}";

                var result = await _http.GetStringAsync(url);
                return JsonSerializer.Deserialize<PlacesResponse>(result);
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
    }
}
