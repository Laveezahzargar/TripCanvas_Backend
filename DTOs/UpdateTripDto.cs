using P6_Travel_Planner_Backend.Enums;

namespace P6_Travel_Planner_Backend.DTOs
{
    public class UpdateTripDto
    {
        public string Title { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public TripStatus Status { get; set; }
    }
}
