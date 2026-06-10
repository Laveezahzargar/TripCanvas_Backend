namespace P6_Travel_Planner_Backend.DTOs
{
    public class CreateItineraryDayDto
    {
        public DateTime Date { get; set; }
        public int DayNumber { get; set; }
        public string? Notes { get; set; }
    }
}
