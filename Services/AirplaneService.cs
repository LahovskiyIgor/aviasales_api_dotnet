using AirlineAPI.Entity;
using AirlineAPI.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirlineAPI.Services
{
    public class AirplaneService : IAirplaneService
    {
        private readonly IAirplaneRepository _repository;

        public AirplaneService(IAirplaneRepository repository)
        {
            _repository = repository;
        }

        public Task<IEnumerable<Airplane>> GetAllAsync() => _repository.GetAllAsync();
        public Task<Airplane> GetByIdAsync(int id) => _repository.GetByIdAsync(id);

        public Task<Airplane> GetAirplaneWithFlightsAsync(int airplaneId) => _repository.GetWithFlightsAsync(airplaneId);
        

        public Task AddAsync(Airplane airplane) => _repository.AddAsync(airplane);
        public Task AddWithSeatsAsync(Airplane airplane) => _repository.AddWithSeatsAsync(airplane);
        public Task UpdateAsync(Airplane airplane) => _repository.UpdateAsync(airplane);
        public Task DeleteAsync(int id) => _repository.DeleteAsync(id);
    }



}
