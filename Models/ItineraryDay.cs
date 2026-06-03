namespace P6_Travel_Planner_Backend.Models
{
    public class ItineraryDay
    {
        public int Id { get; set; }

        public int TripId { get; set; }

        public DateTime Date { get; set; }

        public int DayNumber { get; set; }

        public string Notes { get; set; }

        public Trip Trip { get; set; }

        public ICollection<Activity> Activities { get; set; }
    }
}
