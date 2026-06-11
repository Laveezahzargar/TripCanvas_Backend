using System.Text.Json.Serialization;

namespace P6_Travel_Planner_Backend.DTOs
{
    public class PlacesResponse
    {
        [JsonPropertyName("features")]
        public List<PlaceFeature>? Features { get; set; }
    }
}