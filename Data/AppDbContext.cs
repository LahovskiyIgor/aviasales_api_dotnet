using Microsoft.EntityFrameworkCore;
using AirlineAPI.Entity;

namespace AirlineAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<AirportEntity> Airports { get; set; }
        public DbSet<Airplane> Airplanes { get; set; }
        public DbSet<Flight> Flights { get; set; }
        public DbSet<Passenger> Passengers { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<FlightSeat> FlightSeats { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Связь User ↔ Passenger (один к одному)
            modelBuilder.Entity<User>()
                .HasOne(u => u.Passenger)
                .WithOne(p => p.User)
                .HasForeignKey<Passenger>(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // AirportEntity — связи
            modelBuilder.Entity<Flight>()
               .HasOne(f => f.DepartureAirport)
               .WithMany(a => a.DepartingFlights)
               .HasForeignKey(f => f.DepartureAirportId)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Flight>()
                .HasOne(f => f.ArrivalAirport)
                .WithMany(a => a.ArrivingFlights)
                .HasForeignKey(f => f.ArrivalAirportId)
                .OnDelete(DeleteBehavior.Restrict);

            // Flight → Airplane
            modelBuilder.Entity<Flight>()
                 .HasOne(f => f.Airplane)
                 .WithMany(a => a.Flights)
                 .HasForeignKey(f => f.AirplaneId);


            modelBuilder.Entity<Ticket>()
               .HasOne(t => t.Flight)
               .WithMany(f => f.Tickets)
               .HasForeignKey(t => t.FlightId)
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Passenger)
                .WithMany(p => p.Tickets)
                .HasForeignKey(t => t.PassengerId)
                .OnDelete(DeleteBehavior.Cascade);

            // FlightSeat → Flight
            modelBuilder.Entity<FlightSeat>()
                .HasOne(fs => fs.Flight)
                .WithMany()
                .HasForeignKey(fs => fs.FlightId)
                .OnDelete(DeleteBehavior.Cascade);

            // FlightSeat → Ticket (один к одному, nullable)
            modelBuilder.Entity<FlightSeat>()
                .HasOne(fs => fs.Ticket)
                .WithOne()
                .HasForeignKey<FlightSeat>(fs => fs.TicketId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
