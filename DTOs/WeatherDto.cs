namespace P6_Travel_Planner_Backend.DTOs
{
    public class WeatherDto
    {
        public DateTime Date { get; set; }
        public double Temperature { get; set; }
        public string Condition { get; set; }
        public double Humidity { get; set; }
        public double WindSpeed { get; set; }
    }
}
