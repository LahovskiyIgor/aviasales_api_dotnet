using AirlineAPI.Entity;

namespace AirlineAPI.Interfaces.Repositories
{
    public interface IAirplaneRepository
    {
        Task<IEnumerable<Airplane>> GetAllAsync();
        Task<Airplane> GetByIdAsync(int id);
        Task<Airplane> GetWithFlightsAsync(int airplaneId);

        Task AddAsync(Airplane airplane);
        Task UpdateAsync(Airplane airplane);
        Task DeleteAsync(int id);
    }
}
