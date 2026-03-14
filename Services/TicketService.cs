using AirlineAPI.Entity;
using AirlineAPI.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirlineAPI.Services
{
    public class TicketService : ITicketService
    {
        private readonly ITicketRepository _repository;

        public TicketService(ITicketRepository repository)
        {
            _repository = repository;
        }

        public Task<IEnumerable<Ticket>> GetAllAsync() => _repository.GetAllAsync();
        public Task<Ticket> GetByIdAsync(int id) => _repository.GetByIdAsync(id);
        public Task AddAsync(Ticket ticket) => _repository.AddAsync(ticket);
        public Task UpdateAsync(Ticket ticket) => _repository.UpdateAsync(ticket);
        public Task DeleteAsync(int id) => _repository.DeleteAsync(id);

        public async Task<bool> UpdateStatusAsync(int ticketId, string status, int passengerId)
        {
            var ticket = await _repository.GetByIdAsync(ticketId);
            if (ticket == null || ticket.PassengerId != passengerId)
                return false;

            // Проверяем допустимые статусы
            var validStatuses = new[] { "Забронирован", "Оплачен", "Отменен" };
            if (!validStatuses.Contains(status))
                return false;

            ticket.BookingStatus = status;
            await _repository.UpdateAsync(ticket);
            return true;
        }

        public async Task<Ticket> GetByPassengerAndIdAsync(int ticketId, int passengerId)
        {
            var ticket = await _repository.GetByIdAsync(ticketId);
            if (ticket?.PassengerId == passengerId)
                return ticket;
            return null;
        }
    }

}
