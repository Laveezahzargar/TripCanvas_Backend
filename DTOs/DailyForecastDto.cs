using System.Text.Json.Serialization;

namespace P6_Travel_Planner_Backend.DTOs
{
    public class DailyForecastDto
    {
        [JsonPropertyName("time")]
        public List<string>? Time { get; set; }

        [JsonPropertyName("temperature_2m_max")]
        public List<double>? TemperatureMax { get; set; }

        [JsonPropertyName("temperature_2m_min")]
        public List<double>? TemperatureMin { get; set; }

        [JsonPropertyName("weathercode")]
        public List<int>? WeatherCode { get; set; }
    }
}
