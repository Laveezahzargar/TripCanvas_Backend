namespace P6_Travel_Planner_Backend.DTOs
{
    public class WeatherResponseDto
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public CurrentWeatherDto? CurrentWeather { get; set; }
    }

}
