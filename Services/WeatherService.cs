using P6_Travel_Planner_Backend.DTOs;
using System.Text.Json;

namespace P6_Travel_Planner_Backend.Services
{
    public class WeatherService
    {
        private readonly HttpClient _http;

        public WeatherService(HttpClient http)
        {
            _http = http;
        }

        public async Task<WeatherResponseDto?> GetWeather(double lat, double lon)
        {
            try
            {
                var url = $"https://api.open-meteo.com/v1/forecast?latitude={lat}&longitude={lon}&current_weather=true";

                var response = await _http.GetStringAsync(url);

                return JsonSerializer.Deserialize<WeatherResponseDto>(
                response);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<ForecastResponseDto?> GetForecast(
    double lat,
    double lon)
        {
            try
            {
                var url =
                    $"https://api.open-meteo.com/v1/forecast?latitude={lat}&longitude={lon}&daily=temperature_2m_max,temperature_2m_min&forecast_days=7";

                var response = await _http.GetStringAsync(url);

                return JsonSerializer.Deserialize<ForecastResponseDto>(response);
            }
            catch
            {
                return null;
            }
        }
    }
}
