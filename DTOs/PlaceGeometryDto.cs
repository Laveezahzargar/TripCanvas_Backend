using System.Text.Json.Serialization;

namespace P6_Travel_Planner_Backend.DTOs
{
    public class PlaceGeometry
    {
        [JsonPropertyName("coordinates")]
        public List<double>? Coordinates { get; set; }
    }
}
