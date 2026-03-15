using AirlineAPI.Entity;

namespace AirlineAPI.Interfaces.Repositories
{
    public interface IFlightRepository
    {
        Task<IEnumerable<Flight>> GetAllAsync();
        Task<Flight> GetByIdAsync(int id);

        Task<Flight?> GetFlightDetailedAsync(int flightId);
        Task<Flight?> GetByIdWithAirplaneAsync(int flightId);

        Task AddAsync(Flight flight);
        Task UpdateAsync(Flight flight);
        Task DeleteAsync(int id);
    }
}
