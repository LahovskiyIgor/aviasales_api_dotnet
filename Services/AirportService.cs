using AirlineAPI.Entity;
using AirlineAPI.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirlineAPI.Services
{
    public class AirportService : IAirportService
    {
        private readonly IAirportRepository _repository;

        public AirportService(IAirportRepository repository)
        {
            _repository = repository;
        }

        public Task<IEnumerable<AirportEntity>> GetAllAsync() => _repository.GetAllAsync();
        public Task<AirportEntity> GetByIdAsync(int id) => _repository.GetByIdAsync(id);
        public Task AddAsync(AirportEntity airport) => _repository.AddAsync(airport);
        public Task UpdateAsync(AirportEntity airport) => _repository.UpdateAsync(airport);
        public Task DeleteAsync(int id) => _repository.DeleteAsync(id);
    }

}
