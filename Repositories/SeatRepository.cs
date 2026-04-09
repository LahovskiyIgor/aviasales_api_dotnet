using AirlineAPI.Entity;
using AirlineAPI.Interfaces.Repositories;
using AirlineAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace AirlineAPI.Repositories
{
    public class SeatRepository : ISeatRepository
    {
        private readonly AppDbContext _context;
        public SeatRepository(AppDbContext context) => _context = context;

        public async Task<IEnumerable<Seat>> GetAllAsync() => await _context.Seats.ToListAsync();
        
        public async Task<Seat> GetByIdAsync(int id) => 
            await _context.Seats.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
        
        public async Task<IEnumerable<Seat>> GetByAirplaneIdAsync(int airplaneId) =>
            await _context.Seats.Where(s => s.AirplaneId == airplaneId).ToListAsync();
        
        public async Task AddAsync(Seat seat) 
        { 
            _context.Seats.Add(seat); 
            await _context.SaveChangesAsync(); 
        }
        
        public async Task UpdateAsync(Seat seat) 
        { 
            _context.Seats.Update(seat); 
            await _context.SaveChangesAsync(); 
        }
        
        public async Task DeleteAsync(int id) 
        { 
            var entity = await _context.Seats.FindAsync(id); 
            if (entity != null) 
            { 
                _context.Seats.Remove(entity); 
                await _context.SaveChangesAsync(); 
            } 
        }
    }
}
