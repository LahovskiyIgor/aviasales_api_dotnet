using AirlineAPI.Entity;
using AirlineAPI.Interfaces.Repositories;
using AirlineAPI.Data;
using Microsoft.EntityFrameworkCore;


namespace AirlineAPI.Repositories
{
    public class AirportRepository : IAirportRepository
    {
        private readonly AppDbContext _context;

        public AirportRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AirportEntity>> GetAllAsync() =>
            await _context.Airports.ToListAsync();

        public async Task<AirportEntity> GetByIdAsync(int id) =>
            await _context.Airports.AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id);

        public async Task AddAsync(AirportEntity airport)
        {
            _context.Airports.Add(airport);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(AirportEntity airport)
        {
            _context.Airports.Update(airport);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var airport = await _context.Airports.FindAsync(id);
            if (airport != null)
            {
                _context.Airports.Remove(airport);
                await _context.SaveChangesAsync();
            }
        }
    }
}
