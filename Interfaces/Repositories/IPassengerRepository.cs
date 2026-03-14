using AirlineAPI.Entity;

namespace AirlineAPI.Interfaces.Repositories
{
    public interface IPassengerRepository
    {
        Task<IEnumerable<Passenger>> GetAllAsync();
        Task<Passenger> GetByIdAsync(int id);

        Task<Passenger?> GetPassengerWithTicketsAndFlightsAsync(int passengerId);

        Task AddAsync(Passenger passenger);
        Task UpdateAsync(Passenger passenger);
        Task DeleteAsync(int id);
    }
}
