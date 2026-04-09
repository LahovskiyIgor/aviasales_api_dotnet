using AirlineAPI.Entity;

namespace AirlineAPI.Interfaces.Repositories
{
    public interface ISeatRepository
    {
        Task<IEnumerable<Seat>> GetAllAsync();
        Task<Seat> GetByIdAsync(int id);
        Task<IEnumerable<Seat>> GetByAirplaneIdAsync(int airplaneId);
        Task AddAsync(Seat seat);
        Task UpdateAsync(Seat seat);
        Task DeleteAsync(int id);
    }
}
