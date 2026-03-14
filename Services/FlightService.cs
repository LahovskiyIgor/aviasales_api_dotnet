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
    public class FlightService : IFlightService
    {
        private readonly IFlightRepository _repository;

        public FlightService(IFlightRepository repository)
        {
            _repository = repository;
        }

        public Task<Flight?> GetFlightWithDetailsAsync(int flightId)
        {
            return _repository.GetFlightDetailedAsync(flightId);
        }



        public Task<IEnumerable<Flight>> GetAllAsync() => _repository.GetAllAsync();
        public Task<Flight> GetByIdAsync(int id) => _repository.GetByIdAsync(id);
        public Task AddAsync(Flight flight) => _repository.AddAsync(flight);
        public Task UpdateAsync(Flight flight) => _repository.UpdateAsync(flight);
        public Task DeleteAsync(int id) => _repository.DeleteAsync(id);
    }


}
