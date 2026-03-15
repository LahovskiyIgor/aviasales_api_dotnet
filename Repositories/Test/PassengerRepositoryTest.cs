using AirlineAPI.Interfaces.Repositories;
using AirlineAPI.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirlineAPI.Repositories.Test
{

    public class PassengerRepositoryTest
    {
        private readonly List<Passenger> _passengers = new();
        private int _nextId = 1;

        public PassengerRepositoryTest()
        {
            _passengers.AddRange(new[]
            {
            new Passenger
            {
                Id = _nextId++, FirstName = "Иван", LastName = "Иванов",
                Email = "ivan@mail.com", Phone = "89001112233", Password = "1234"
            },
            new Passenger
            {
                Id = _nextId++, FirstName = "Елена", LastName = "Смирнова",
                Email = "elena@mail.com", Phone = "89004445566", Password = "5678"
            }
        });
        }

        public Task<IEnumerable<Passenger>> GetAllAsync() => Task.FromResult(_passengers.AsEnumerable());

        public Task<Passenger> GetByIdAsync(int id) =>
            Task.FromResult(_passengers.FirstOrDefault(p => p.Id == id));

        public Task AddAsync(Passenger entity)
        {
            entity.Id = _nextId++;
            _passengers.Add(entity);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Passenger entity)
        {
            var existing = _passengers.FirstOrDefault(p => p.Id == entity.Id);
            if (existing != null)
            {
                existing.FirstName = entity.FirstName;
                existing.LastName = entity.LastName;
                existing.Email = entity.Email;
                existing.Phone = entity.Phone;
                existing.Password = entity.Password;
            }
            return Task.CompletedTask;
        }

        public Task DeleteAsync(int id)
        {
            var passenger = _passengers.FirstOrDefault(p => p.Id == id);
            if (passenger != null) _passengers.Remove(passenger);
            return Task.CompletedTask;
        }

        public Task<Passenger?> GetPassengerWithTicketsAndFlightsAsync(int passengerId)
        {
            throw new NotImplementedException();
        }
    }


}
