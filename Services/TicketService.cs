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
        private readonly IFlightRepository _flightRepository;

        public TicketService(ITicketRepository repository, IFlightRepository flightRepository)
        {
            _repository = repository;
            _flightRepository = flightRepository;
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
            var validStatuses = new[] { "Зарезервирован", "Оплачен", "Отменен" };
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

        public async Task<Ticket> ReserveTicketAsync(int flightId, int passengerId, string seatNumber)
        {
            // Проверяем существование рейса
            var flight = await _flightRepository.GetByIdAsync(flightId);
            if (flight == null)
                throw new ArgumentException("Рейс не найден");

            // Проверяем, не занято ли место
            var existingTickets = await _repository.GetAllAsync();
            var seatTaken = existingTickets.Any(t => 
                t.FlightId == flightId && 
                t.SeatNumber == seatNumber && 
                t.BookingStatus is "Зарезервирован" or "Оплачен");
            
            if (seatTaken)
                throw new InvalidOperationException("Место уже занято");

            // Создаём резервирование
            var ticket = new Ticket
            {
                FlightId = flightId,
                PassengerId = passengerId,
                SeatNumber = seatNumber,
                BookingStatus = "Зарезервирован"
            };

            await _repository.AddAsync(ticket);
            
            // Обновляем счётчик зарезервированных мест
            flight.ReservedTickets++;
            await _flightRepository.UpdateAsync(flight);

            return ticket;
        }

        public async Task<bool> CancelReservationAsync(int ticketId, int passengerId)
        {
            var ticket = await _repository.GetByIdAsync(ticketId);
            if (ticket == null || ticket.PassengerId != passengerId)
                return false;

            if (ticket.BookingStatus != "Зарезервирован")
                return false;

            ticket.BookingStatus = "Отменен";
            await _repository.UpdateAsync(ticket);

            // Обновляем счётчик зарезервированных мест
            var flight = await _flightRepository.GetByIdAsync(ticket.FlightId);
            if (flight != null && flight.ReservedTickets > 0)
            {
                flight.ReservedTickets--;
                await _flightRepository.UpdateAsync(flight);
            }

            return true;
        }

        public async Task CancelExpiredReservationsAsync()
        {
            // Метод устарел после удаления ReservationExpiresAt
            // Резервации теперь не имеют автоматического времени истечения
            await Task.CompletedTask;
        }
    }

}
