using System.Text.Json.Serialization;

namespace P6_Travel_Planner_Backend.DTOs
{
    public class ForecastResponseDto
    {
        public double Latitude { get; set; }

        public double Longitude { get; set; }

        [JsonPropertyName("daily")]
        public DailyForecastDto? Daily { get; set; }
    }
}
