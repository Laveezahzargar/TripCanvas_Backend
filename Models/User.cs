using System.ComponentModel.DataAnnotations;
using P6_Travel_Planner_Backend.Enums;

namespace P6_Travel_Planner_Backend.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public Role Role { get; set; } = Role.User;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Trip> Trips { get; set; }
    }
}
