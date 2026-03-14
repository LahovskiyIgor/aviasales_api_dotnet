using AirlineAPI.Interfaces.Repositories;
using AirlineAPI.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirlineAPI.Repositories.Test
{

    public class FlightRepositoryTest : IFlightRepository
    {
        private readonly List<Flight> _flights = new();
        private int _nextId = 1;

        public FlightRepositoryTest()
        {
            _flights.AddRange(new[]
            {
            new Flight
            {
                Id = _nextId++,
                FlightNumber = "SU1001",
                DepartureAirportId = 1,
                ArrivalAirportId = 2,
                DepartureTime = DateTime.Now.AddHours(3),
                ArrivalTime = DateTime.Now.AddHours(5),
                AirplaneId = 1,
                TotalSeats = 160,
                SoldTickets = 10,
                ReservedTickets = 5
            },
            new Flight
            {
                Id = _nextId++,
                FlightNumber = "BA456",
                DepartureAirportId = 2,
                ArrivalAirportId = 3,
                DepartureTime = DateTime.Now.AddDays(1),
                ArrivalTime = DateTime.Now.AddDays(1).AddHours(3),
                AirplaneId = 2,
                TotalSeats = 180,
                SoldTickets = 50,
                ReservedTickets = 10
            }
        });
        }

        public Task<IEnumerable<Flight>> GetAllAsync() => Task.FromResult(_flights.AsEnumerable());

        public Task<Flight> GetByIdAsync(int id) =>
            Task.FromResult(_flights.FirstOrDefault(f => f.Id == id));

        public Task AddAsync(Flight entity)
        {
            entity.Id = _nextId++;
            _flights.Add(entity);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Flight entity)
        {
            var existing = _flights.FirstOrDefault(f => f.Id == entity.Id);
            if (existing != null)
            {
                existing.FlightNumber = entity.FlightNumber;
                existing.DepartureAirportId = entity.DepartureAirportId;
                existing.ArrivalAirportId = entity.ArrivalAirportId;
                existing.DepartureTime = entity.DepartureTime;
                existing.ArrivalTime = entity.ArrivalTime;
                existing.AirplaneId = entity.AirplaneId;
                existing.TotalSeats = entity.TotalSeats;
                existing.SoldTickets = entity.SoldTickets;
                existing.ReservedTickets = entity.ReservedTickets;
            }
            return Task.CompletedTask;
        }

        public Task DeleteAsync(int id)
        {
            var flight = _flights.FirstOrDefault(f => f.Id == id);
            if (flight != null) _flights.Remove(flight);
            return Task.CompletedTask;
        }

        public Task<Flight?> GetFlightDetailedAsync(int flightId)
        {
            throw new NotImplementedException();
        }
    }


}
