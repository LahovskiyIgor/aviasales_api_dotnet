using AirlineAPI.Interfaces.Repositories;
using AirlineAPI.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirlineAPI.Repositories.Test
{

    public class AirplaneRepositoryTest : IAirplaneRepository
    {
        private readonly List<Airplane> _airplanes = new();
        private int _nextId = 1;

        public AirplaneRepositoryTest()
        {
            _airplanes.AddRange(new[]
            {
            new Airplane { Id = _nextId++, Model = "Boeing 737", Capacity = 160 },
            new Airplane { Id = _nextId++, Model = "Airbus A320", Capacity = 180 }
        });
        }

        public Task<IEnumerable<Airplane>> GetAllAsync() => Task.FromResult(_airplanes.AsEnumerable());

        public Task<Airplane> GetByIdAsync(int id) =>
            Task.FromResult(_airplanes.FirstOrDefault(a => a.Id == id));

        public Task AddAsync(Airplane entity)
        {
            entity.Id = _nextId++;
            _airplanes.Add(entity);
            Console.WriteLine(_airplanes.Count);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Airplane entity)
        {
            var existing = _airplanes.FirstOrDefault(a => a.Id == entity.Id);
            if (existing != null)
            {
                existing.Model = entity.Model;
                existing.Capacity = entity.Capacity;
            }
            return Task.CompletedTask;
        }

        public Task DeleteAsync(int id)
        {
            var airplane = _airplanes.FirstOrDefault(a => a.Id == id);
            if (airplane != null) _airplanes.Remove(airplane);
            return Task.CompletedTask;

        }

        public Task<Airplane> GetWithFlightsAsync(int airplaneId)
        {
            throw new NotImplementedException();
        }
    }


}
