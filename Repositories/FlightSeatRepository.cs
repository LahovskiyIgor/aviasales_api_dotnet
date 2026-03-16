using AirlineAPI.Entity;
using AirlineAPI.Interfaces.Repositories;
using AirlineAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace AirlineAPI.Repositories
{
    public class FlightSeatRepository : IFlightSeatRepository
    {
        private readonly AppDbContext _context;
        
        public FlightSeatRepository(AppDbContext context) => _context = context;

        public async Task<IEnumerable<FlightSeat>> GetAllAsync() => 
            await _context.FlightSeats.ToListAsync();
            
        public async Task<FlightSeat> GetByIdAsync(int id) => 
            await _context.FlightSeats.FindAsync(id);
            
        public async Task<IEnumerable<FlightSeat>> GetByFlightIdAsync(int flightId) => 
            await _context.FlightSeats
                .Where(fs => fs.FlightId == flightId)
                .ToListAsync();
                
        public async Task<FlightSeat?> GetByFlightAndSeatNumberAsync(int flightId, string seatNumber) => 
            await _context.FlightSeats
                .FirstOrDefaultAsync(fs => fs.FlightId == flightId && fs.SeatNumber == seatNumber);

        public async Task AddAsync(FlightSeat flightSeat) 
        { 
            _context.FlightSeats.Add(flightSeat); 
            await _context.SaveChangesAsync(); 
        }
        
        public async Task UpdateAsync(FlightSeat flightSeat) 
        { 
            _context.FlightSeats.Update(flightSeat); 
            await _context.SaveChangesAsync(); 
        }
        
        public async Task DeleteAsync(int id) 
        { 
            var entity = await _context.FlightSeats.FindAsync(id); 
            if (entity != null) 
            { 
                _context.FlightSeats.Remove(entity); 
                await _context.SaveChangesAsync(); 
            } 
        }
        
        public async Task InitializeSeatsForFlightAsync(int flightId, int capacity)
        {
            // Проверяем, существуют ли уже места для этого рейса
            var existingSeats = await GetByFlightIdAsync(flightId);
            if (existingSeats.Any())
                return;

            // Генерируем места на основе вместимости самолёта
            int rows = capacity / 4; // 4 места в ряду (A, B, C, D)
            char[] seatLetters = { 'A', 'B', 'C', 'D' };

            for (int row = 1; row <= rows; row++)
            {
                foreach (var letter in seatLetters)
                {
                    var seat = new FlightSeat
                    {
                        FlightId = flightId,
                        SeatNumber = $"{row}{letter}",
                        IsAvailable = true
                    };
                    await AddAsync(seat);
                }
            }
        }
    }
}
