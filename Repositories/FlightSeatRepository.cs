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
            
        public async Task<FlightSeat?> GetByIdAsync(int id) => 
            await _context.FlightSeats.FindAsync(id);

        public async Task<IEnumerable<FlightSeat>> GetByFlightIdAsync(int flightId) =>
            await _context.FlightSeats
                .Where(fs => fs.FlightId == flightId)
                .OrderBy(fs => fs.SeatNumber)
                .ToListAsync();

        public async Task<FlightSeat?> GetByFlightAndSeatNumberAsync(int flightId, string seatNumber) =>
            await _context.FlightSeats
                .FirstOrDefaultAsync(fs => fs.FlightId == flightId && fs.SeatNumber == seatNumber.ToUpper());

        public async Task AddAsync(FlightSeat seat)
        {
            _context.FlightSeats.Add(seat);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(FlightSeat seat)
        {
            _context.FlightSeats.Update(seat);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var seat = await _context.FlightSeats.FindAsync(id);
            if (seat != null)
            {
                _context.FlightSeats.Remove(seat);
                await _context.SaveChangesAsync();
            }
        }

        public async Task InitializeSeatsForFlightAsync(int flightId, int capacity)
        {
            // Проверяем, существуют ли уже места для этого рейса
            var existingSeats = await GetByFlightIdAsync(flightId);
            if (existingSeats.Any())
                return;

            // Генерируем места на основе вместимости (4 места в ряду: A, B, C, D)
            int rows = capacity / 4;
            char[] seatLetters = { 'A', 'B', 'C', 'D' };

            var seats = new List<FlightSeat>();
            for (int row = 1; row <= rows; row++)
            {
                foreach (var letter in seatLetters)
                {
                    seats.Add(new FlightSeat
                    {
                        FlightId = flightId,
                        SeatNumber = $"{row}{letter}",
                        Status = SeatStatus.Available
                    });
                }
            }

            _context.FlightSeats.AddRange(seats);
            await _context.SaveChangesAsync();
        }
    }
}
