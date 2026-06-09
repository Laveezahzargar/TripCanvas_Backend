namespace P6_Travel_Planner_Backend.DTOs
{
    public class ItineraryDayDto
    {
        public int Id { get; set; }
        public int TripId { get; set; }
        public DateTime Date { get; set; }
        public int DayNumber { get; set; }
        public string Notes { get; set; }
    }
}
