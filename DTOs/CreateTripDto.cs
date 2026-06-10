namespace P6_Travel_Planner_Backend.DTOs
{
    public class CreateTripDto
    {
        public string Title { get; set; }
        public int DestinationId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
