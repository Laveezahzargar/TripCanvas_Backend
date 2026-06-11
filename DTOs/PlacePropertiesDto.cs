using System.Text.Json.Serialization;

namespace P6_Travel_Planner_Backend.DTOs
{
    public class PlaceProperties
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("formatted")]
        public string? Address { get; set; }

        [JsonPropertyName("categories")]
        public List<string>? Categories { get; set; }
    }
}
