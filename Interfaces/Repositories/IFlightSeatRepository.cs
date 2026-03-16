using AirlineAPI.Entity;

namespace AirlineAPI.Interfaces.Repositories
{
    public interface IFlightSeatRepository
    {
        Task<IEnumerable<FlightSeat>> GetAllAsync();
        Task<FlightSeat?> GetByIdAsync(int id);
        Task<IEnumerable<FlightSeat>> GetByFlightIdAsync(int flightId);
        Task<FlightSeat?> GetByFlightAndSeatNumberAsync(int flightId, string seatNumber);
        Task AddAsync(FlightSeat seat);
        Task UpdateAsync(FlightSeat seat);
        Task DeleteAsync(int id);
        Task InitializeSeatsForFlightAsync(int flightId, int capacity);
    }
}
