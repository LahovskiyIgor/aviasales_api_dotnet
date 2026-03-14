using AirlineAPI.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirlineAPI.Services
{
    public interface IAirportService
    {
        Task<IEnumerable<AirportEntity>> GetAllAsync();
        Task<AirportEntity> GetByIdAsync(int id);
        Task AddAsync(AirportEntity airport);
        Task UpdateAsync(AirportEntity airport);
        Task DeleteAsync(int id);
    }


}
