using AirlineAPI.Entity;

namespace AirlineAPI.Interfaces.Repositories
{
    public interface IAirportRepository
    {
        Task<IEnumerable<AirportEntity>> GetAllAsync();
        Task<AirportEntity> GetByIdAsync(int id);
        Task AddAsync(AirportEntity airport);
        Task UpdateAsync(AirportEntity airport);
        Task DeleteAsync(int id);
    }
}
