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
        private readonly ISeatService _seatService;

        public FlightService(IFlightRepository repository, ISeatService seatService)
        {
            _repository = repository;
            _seatService = seatService;
        }

        public Task<Flight?> GetFlightWithDetailsAsync(int flightId)
        {
            return _repository.GetFlightDetailedAsync(flightId);
        }

        public async Task AddAsync(Flight flight)
        {
            await _repository.AddAsync(flight);
            // После создания рейса инициализируем места
            await _seatService.InitializeSeatsForFlightAsync(flight.Id);
        }
        
        public Task UpdateAsync(Flight flight) => _repository.UpdateAsync(flight);
        public Task DeleteAsync(int id) => _repository.DeleteAsync(id);
    }


}
