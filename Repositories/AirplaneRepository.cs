using AirlineAPI.Entity;
using AirlineAPI.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using AirlineAPI.Data;


namespace AirlineAPI.Repositories
{
    public class AirplaneRepository : IAirplaneRepository
    {
        private readonly AppDbContext _context;
        public AirplaneRepository(AppDbContext context) => _context = context;

        public async Task<IEnumerable<Airplane>> GetAllAsync() => await _context.Airplanes.ToListAsync();
        public async Task<Airplane> GetByIdAsync(int id) => 
            await _context.Airplanes.AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id);


        public async Task<Airplane> GetWithFlightsAsync(int airplaneId)
        {
            return await _context.Airplanes
                .Include(a => a.Flights)
                .AsSplitQuery()
                .FirstOrDefaultAsync(a => a.Id == airplaneId);
        }

        public async Task AddAsync(Airplane airplane) { _context.Airplanes.Add(airplane); await _context.SaveChangesAsync(); }
        
        public async Task AddWithSeatsAsync(Airplane airplane)
        {
            // 1. Сохраняем самолет
            _context.Airplanes.Add(airplane);
            await _context.SaveChangesAsync();

            // 2. Генерируем список мест в коде
            var seats = Enumerable.Range(1, airplane.Capacity).Select(n => new Seat
            {
                AirplaneId = airplane.Id,
                SeatNumber = $"{(n - 1) / 6 + 1}{ "ABCDEF"[(n - 1) % 6] }",
                Sector = n <= 12 ? "Бизнес" : "Эконом",
                PriceMultiplier = n <= 12 ? 2.0m : 1.0m
            }).ToList();

            // 3. Сохраняем места
            _context.Seats.AddRange(seats);
            await _context.SaveChangesAsync();
        }
        
        public async Task UpdateAsync(Airplane airplane) { _context.Airplanes.Update(airplane); await _context.SaveChangesAsync(); }
        public async Task DeleteAsync(int id) { var entity = await _context.Airplanes.FindAsync(id); if (entity != null) { _context.Airplanes.Remove(entity); await _context.SaveChangesAsync(); } }
    }
}
