using AirlineAPI.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirlineAPI.Services
{
    public interface IAirplaneService
    {
        Task<IEnumerable<Airplane>> GetAllAsync();
        Task<Airplane> GetByIdAsync(int id);

        Task<Airplane> GetAirplaneWithFlightsAsync(int airplaneId);

        Task AddAsync(Airplane airplane);
        Task UpdateAsync(Airplane airplane);
        Task DeleteAsync(int id);
    }


}
