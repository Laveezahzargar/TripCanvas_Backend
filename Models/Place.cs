namespace P6_Travel_Planner_Backend.Models
{
    public class Place
    {
        public int Id { get; set; }

        public int DestinationId { get; set; }

        public string Name { get; set; }

        public string Category { get; set; }

        public double Rating { get; set; }

        public string Location { get; set; }

        public string ImageUrl { get; set; }

        public Destination Destination { get; set; }
    }
}
