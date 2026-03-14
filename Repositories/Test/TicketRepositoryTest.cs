using AirlineAPI.Interfaces.Repositories;
using AirlineAPI.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirlineAPI.Repositories.Test
{

    public class TicketRepositoryTest : ITicketRepository
    {
        private readonly List<Ticket> _tickets = new();
        private int _nextId = 1;

        public TicketRepositoryTest()
        {
            _tickets.AddRange(new[]
            {
            new Ticket
            {
                Id = _nextId++, FlightId = 1, PassengerId = 1,
                BookingStatus = "оплачен", SeatNumber = "12A"
            },
            new Ticket
            {
                Id = _nextId++, FlightId = 2, PassengerId = 2,
                BookingStatus = "забронирован", SeatNumber = "15B"
            }
        });
        }

        public Task<IEnumerable<Ticket>> GetAllAsync() => Task.FromResult(_tickets.AsEnumerable());

        public Task<Ticket> GetByIdAsync(int id) =>
            Task.FromResult(_tickets.FirstOrDefault(t => t.Id == id));

        public Task AddAsync(Ticket entity)
        {
            entity.Id = _nextId++;
            _tickets.Add(entity);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Ticket entity)
        {
            var existing = _tickets.FirstOrDefault(t => t.Id == entity.Id);
            if (existing != null)
            {
                existing.FlightId = entity.FlightId;
                existing.PassengerId = entity.PassengerId;
                existing.BookingStatus = entity.BookingStatus;
                existing.SeatNumber = entity.SeatNumber;
            }
            return Task.CompletedTask;
        }

        public Task DeleteAsync(int id)
        {
            var ticket = _tickets.FirstOrDefault(t => t.Id == id);
            if (ticket != null) _tickets.Remove(ticket);
            return Task.CompletedTask;
        }
    }


}
