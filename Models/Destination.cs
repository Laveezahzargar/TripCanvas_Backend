namespace P6_Travel_Planner_Backend.Models
{
    public class Destination
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Country { get; set; }

        public string Description { get; set; }

        public string ImageUrl { get; set; }

        public double Latitude { get; set; } = 0;
        public double Longitude { get; set; } = 0;

        public ICollection<Trip> Trips { get; set; }
        public ICollection<Place> Places { get; set; }
        public ICollection<Weather> WeatherData { get; set; }
    }
}
