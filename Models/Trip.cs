using P6_Travel_Planner_Backend.Enums;

namespace P6_Travel_Planner_Backend.Models
{
    public class Trip
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public int DestinationId { get; set; }

        public string Title { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public TripStatus Status { get; set; } = TripStatus.InProgress;

        public User User { get; set; }
        public Destination Destination { get; set; }

        public ICollection<ItineraryDay> Days { get; set; }
    }
}
