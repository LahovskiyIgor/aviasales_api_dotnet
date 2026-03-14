using AirlineAPI.Interfaces.Repositories;
using AirlineAPI.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirlineAPI.Repositories.Test
{
    

    public class AirportRepositoryTest : IAirportRepository
    {
        private readonly List<AirportEntity> _airports = new();
        private int _nextId = 1;

        public AirportRepositoryTest()
        {
            // Добавим тестовые данные
            _airports.AddRange(new[]
            {
            new AirportEntity { Id = _nextId++, Name = "Домодедово", Location = "Москва" },
            new AirportEntity { Id = _nextId++, Name = "Пулково", Location = "Санкт Петербург" },
            new AirportEntity { Id = _nextId++, Name = "Heathrow", Location = "Лондон" }
        });
        }

        public Task<IEnumerable<AirportEntity>> GetAllAsync() => Task.FromResult(_airports.AsEnumerable());

        public Task<AirportEntity> GetByIdAsync(int id) =>
            Task.FromResult(_airports.FirstOrDefault(a => a.Id == id));

        public Task AddAsync(AirportEntity entity)
        {
            entity.Id = _nextId++;
            _airports.Add(entity);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(AirportEntity entity)
        {
            var existing = _airports.FirstOrDefault(a => a.Id == entity.Id);
            if (existing != null)
            {
                existing.Name = entity.Name;
                existing.Location = entity.Location;
            }
            return Task.CompletedTask;
        }

        public Task DeleteAsync(int id)
        {
            var airport = _airports.FirstOrDefault(a => a.Id == id);
            if (airport != null) _airports.Remove(airport);
            return Task.CompletedTask;
        }
    }

}
