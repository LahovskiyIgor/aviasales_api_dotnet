using AirlineAPI.Entity;
using AirlineAPI.Interfaces.Repositories;
using AirlineAPI.Data;
using Microsoft.EntityFrameworkCore;



namespace AirlineAPI.Repositories
{
    public class PassengerRepository : IPassengerRepository
    {
        private readonly AppDbContext _context;
        public PassengerRepository(AppDbContext context) => _context = context;

        public async Task<IEnumerable<Passenger>> GetAllAsync() => await _context.Passengers.ToListAsync();
        public async Task<Passenger> GetByIdAsync(int id) => 
            await _context.Passengers.AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id);

        public async Task<Passenger?> GetPassengerWithTicketsAndFlightsAsync(int passengerId)
        {
            return await _context.Passengers
                .Include(p => p.Tickets)
                    .ThenInclude(t => t.Flight)
                        .ThenInclude(f => f.DepartureAirport)
                .Include(p => p.Tickets)
                    .ThenInclude(t => t.Flight)
                        .ThenInclude(f => f.ArrivalAirport)
                .Include(p => p.Tickets)
                    .ThenInclude(t => t.Flight)
                        .ThenInclude(f => f.Airplane)
                .FirstOrDefaultAsync(p => p.Id == passengerId);
        }

        public async Task AddAsync(Passenger passenger) { _context.Passengers.Add(passenger); await _context.SaveChangesAsync(); }
        public async Task UpdateAsync(Passenger passenger) { _context.Passengers.Update(passenger); await _context.SaveChangesAsync(); }
        public async Task DeleteAsync(int id) { var entity = await _context.Passengers.FindAsync(id); if (entity != null) { _context.Passengers.Remove(entity); await _context.SaveChangesAsync(); } }
    }
}
