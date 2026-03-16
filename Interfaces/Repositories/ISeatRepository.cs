using AirlineAPI.Entity;

namespace AirlineAPI.Interfaces.Repositories
{
    public interface ISeatRepository
    {
        Task<IEnumerable<Seat>> GetAllAsync();
        Task<Seat> GetByIdAsync(int id);
        Task<IEnumerable<Seat>> GetByFlightIdAsync(int flightId);
        Task AddAsync(Seat seat);
        Task AddRangeAsync(IEnumerable<Seat> seats);
        Task UpdateAsync(Seat seat);
        Task DeleteAsync(int id);
    }
}
