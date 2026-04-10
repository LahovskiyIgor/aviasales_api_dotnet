using AirlineAPI.Entity;
using AirlineAPI.Interfaces.Repositories;
using AirlineAPI.Data;
using Microsoft.EntityFrameworkCore;


namespace AirlineAPI.Repositories
{
    public class FlightRepository : IFlightRepository 
    {
        private readonly AppDbContext _context;
        public FlightRepository(AppDbContext context) => _context = context;

        public async Task<IEnumerable<Flight>> GetAllAsync() => await _context.Flights
            .Include(f => f.DepartureAirport)
            .Include(f => f.ArrivalAirport)
            .Include(f => f.Airplane)
                .ThenInclude(a => a.Seats)
            .Include(f => f.Tickets)
                .ThenInclude(t => t.Seat)
            .AsSplitQuery()
            .ToListAsync();
        public async Task<Flight> GetByIdAsync(int id) =>
            await _context.Flights
            .Include(f => f.DepartureAirport)
            .Include(f => f.ArrivalAirport)
            .Include(f => f.Airplane)
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id);

        public async Task<Flight?> GetFlightDetailedAsync(int flightId)
        {
            return await _context.Flights
                .Include(f => f.DepartureAirport)
                .Include(f => f.ArrivalAirport)
                .Include(f => f.Airplane)
                    .ThenInclude(a => a.Seats)
                .Include(f => f.Tickets)
                    .ThenInclude(t => t.Seat)
                .AsSplitQuery()
                .FirstOrDefaultAsync(f => f.Id == flightId);
        }

        public async Task<Flight?> GetByIdWithAirplaneAsync(int flightId)
        {
            return await _context.Flights
                .Include(f => f.Airplane)
                .FirstOrDefaultAsync(f => f.Id == flightId);
        }

        public async Task<Flight?> GetByIdWithAirplaneAndSeatsAsync(int flightId)
        {
            return await _context.Flights
                .Include(f => f.Airplane)
                    .ThenInclude(a => a.Seats)
                .AsSplitQuery()
                .FirstOrDefaultAsync(f => f.Id == flightId);
        }

        public async Task AddAsync(Flight flight) { _context.Flights.Add(flight); await _context.SaveChangesAsync(); }
        public async Task UpdateAsync(Flight flight) { _context.Flights.Update(flight); await _context.SaveChangesAsync(); }
        public async Task DeleteAsync(int id) { var entity = await _context.Flights.FindAsync(id); if (entity != null) { _context.Flights.Remove(entity); await _context.SaveChangesAsync(); } }
    }
}
