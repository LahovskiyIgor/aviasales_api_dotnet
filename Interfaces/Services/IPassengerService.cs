using AirlineAPI.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirlineAPI.Services
{
    public interface IPassengerService
    {
        Task<IEnumerable<Passenger>> GetAllAsync();
        Task<Passenger> GetByIdAsync(int id);
        Task<Passenger?> GetPassengerWithTicketsAndFlightsAsync(int passengerId);

        Task AddAsync(Passenger passenger);
        Task UpdateAsync(Passenger passenger);
        Task DeleteAsync(int id);
    }

}
