using AirlineAPI.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirlineAPI.Services
{
    public interface ISeatService
    {
        Task<IEnumerable<Seat>> GetAllAsync();
        Task<Seat> GetByIdAsync(int id);
        Task<IEnumerable<Seat>> GetByFlightIdAsync(int flightId);
        Task InitializeSeatsForFlightAsync(int flightId);
        Task UpdateSeatStatusAsync(int seatId, bool isAvailable);
        Task DeleteAsync(int id);
    }
}
