using System.Text.Json.Serialization;

namespace P6_Travel_Planner_Backend.DTOs
{
    public class PlaceFeature
    {
        [JsonPropertyName("properties")]
        public PlaceProperties? Properties { get; set; }

        [JsonPropertyName("geometry")]
        public PlaceGeometry? Geometry { get; set; }
    }
}