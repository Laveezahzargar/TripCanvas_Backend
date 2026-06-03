

using Microsoft.EntityFrameworkCore;


using P6_Travel_Planner_Backend.Models;

namespace P6_Travel_Planner_Backend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Destination> Destinations { get; set; }
        public DbSet<Trip> Trips { get; set; }
        public DbSet<ItineraryDay> ItineraryDays { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<Place> Places { get; set; }
        public DbSet<Weather> Weather { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
            .Property(u => u.Email)
            .IsRequired();

            modelBuilder.Entity<User>()
                .Property(u => u.PasswordHash)
                .IsRequired();

            modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

            modelBuilder.Entity<Activity>()
            .Property(a => a.Cost)
            .HasColumnType("decimal(10,2)");

            modelBuilder.Entity<User>()
            .HasMany(u => u.Trips)
            .WithOne(t => t.User)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Destination>()
            .HasMany(d => d.Trips)
            .WithOne(t => t.Destination)
            .HasForeignKey(t => t.DestinationId)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Destination>()
            .HasMany(d => d.Places)
            .WithOne(p => p.Destination)
            .HasForeignKey(p => p.DestinationId)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Destination>()
            .HasMany(d => d.WeatherData)
            .WithOne(w => w.Destination)
            .HasForeignKey(w => w.DestinationId)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Trip>()
            .HasMany(t => t.Days)
            .WithOne(d => d.Trip)
            .HasForeignKey(d => d.TripId)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ItineraryDay>()
            .HasMany(d => d.Activities)
            .WithOne(a => a.ItineraryDay)
            .HasForeignKey(a => a.ItineraryDayId)
            .OnDelete(DeleteBehavior.Cascade);


        }
    }


}
