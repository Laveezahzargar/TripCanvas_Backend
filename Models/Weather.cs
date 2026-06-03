namespace P6_Travel_Planner_Backend.Models
{
    public class Weather
    {
        public int Id { get; set; }

        public int DestinationId { get; set; }

        public double Temperature { get; set; }

        public string Condition { get; set; }

        public int Humidity { get; set; }

        public string ForecastData { get; set; } // JSON stored as string

        public DateTime Date { get; set; }

        public Destination Destination { get; set; }
    }
}
