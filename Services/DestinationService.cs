namespace P6_Travel_Planner_Backend.Services
{
    public class DestinationService
    {
        private readonly HttpClient _http;
        private const string apiKey = "YOUR_KEY";

        public DestinationService(HttpClient http)
        {
            _http = http;
        }

        public async Task<string> GetPlaces(double lat, double lon)
        {
            var url = $"https://api.opentripmap.com/0.1/en/places/radius?radius=5000&lon={lon}&lat={lat}&apikey={apiKey}";

            var result = await _http.GetStringAsync(url);
            return result;
        }
    }
}
