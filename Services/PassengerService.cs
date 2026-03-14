using AirlineAPI.Entity;
using AirlineAPI.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirlineAPI.Services
{
    public class PassengerService : IPassengerService
    {
        private readonly IPassengerRepository _repository;

        public PassengerService(IPassengerRepository repository)
        {
            _repository = repository;
        }

        public Task<Passenger?> GetPassengerWithTicketsAndFlightsAsync(int passengerId)
        {
            return _repository.GetPassengerWithTicketsAndFlightsAsync(passengerId);
        }


        public Task<IEnumerable<Passenger>> GetAllAsync() => _repository.GetAllAsync();
        public Task<Passenger> GetByIdAsync(int id) => _repository.GetByIdAsync(id);
        public Task AddAsync(Passenger passenger) => _repository.AddAsync(passenger);
        public Task UpdateAsync(Passenger passenger) => _repository.UpdateAsync(passenger);
        public Task DeleteAsync(int id) => _repository.DeleteAsync(id);
    }


}
