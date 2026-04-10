using AirlineAPI.Entity;
using AirlineAPI.Interfaces.Repositories;
using AirlineAPI.Interfaces.Services;
using Microsoft.EntityFrameworkCore;

namespace AirlineAPI.Services
{
    public class TicketService : ITicketService
    {
        private readonly ITicketRepository _repository;
        private readonly IFlightRepository _flightRepository;
        private readonly ISeatRepository _seatRepository;

        public TicketService(ITicketRepository repository, IFlightRepository flightRepository, ISeatRepository seatRepository)
        {
            _repository = repository;
            _flightRepository = flightRepository;
            _seatRepository = seatRepository;
        }

        public Task<IEnumerable<Ticket>> GetAllAsync() => _repository.GetAllAsync();
        public Task<IEnumerable<Ticket>> GetAllWithDetailsAsync() => _repository.GetAllWithDetailsAsync();
        public Task<Ticket> GetByIdAsync(int id) => _repository.GetByIdAsync(id);
        public Task AddAsync(Ticket ticket) => _repository.AddAsync(ticket);
        public Task UpdateAsync(Ticket ticket) => _repository.UpdateAsync(ticket);
        public Task DeleteAsync(int id) => _repository.DeleteAsync(id);

        public async Task<bool> UpdateStatusAsync(int ticketId, string status, int passengerId)
        {
            var ticket = await _repository.GetByIdAsync(ticketId);
            if (ticket == null || ticket.PassengerId != passengerId)
                return false;

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

        public async Task<Ticket> ReserveTicketAsync(int flightId, int passengerId, int seatId)
        {
            var flight = await _flightRepository.GetByIdWithAirplaneAndSeatsAsync(flightId);
            if (flight == null)
                throw new ArgumentException("Рейс не найден");
            
            if (flight.Airplane == null)
                throw new ArgumentException("Данные о самолёте недоступны");

            var seat = await _seatRepository.GetByIdAsync(seatId);
            if (seat == null || seat.AirplaneId != flight.AirplaneId)
                throw new ArgumentException("Место не найдено или не принадлежит этому самолёту");

            var existingTicket = await _repository.GetAllAsync();
            var seatTaken = existingTicket.Any(t => 
                t.FlightId == flightId && 
                t.SeatId == seatId && 
                t.BookingStatus is "Зарезервирован" or "Оплачен");
            
            if (seatTaken)
                throw new InvalidOperationException("Место уже занято");

            var ticket = new Ticket
            {
                FlightId = flightId,
                PassengerId = passengerId,
                SeatId = seatId,
                BookingStatus = "Зарезервирован"
            };

            await _repository.AddAsync(ticket);
            
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

            var flight = await _flightRepository.GetByIdAsync(ticket.FlightId);
            if (flight != null && flight.ReservedTickets > 0)
            {
                flight.ReservedTickets--;
                await _flightRepository.UpdateAsync(flight);
            }

            return true;
        }

        public async Task<bool> CancelPaidTicketAsync(int ticketId, int passengerId)
        {
            var ticket = await _repository.GetByIdAsync(ticketId);
            if (ticket == null || ticket.PassengerId != passengerId)
                return false;

            if (ticket.BookingStatus != "Оплачен")
                return false;

            ticket.BookingStatus = "Отменен";
            await _repository.UpdateAsync(ticket);

            var flight = await _flightRepository.GetByIdAsync(ticket.FlightId);
            if (flight != null && flight.SoldTickets > 0)
            {
                flight.SoldTickets--;
                await _flightRepository.UpdateAsync(flight);
            }

            return true;
        }

        public async Task<IEnumerable<Seat>> GetAvailableSeatsAsync(int flightId)
        {
            var flight = await _flightRepository.GetByIdWithAirplaneAndSeatsAsync(flightId);
            if (flight == null)
                throw new ArgumentException("Рейс не найден");
            
            if (flight.Airplane == null)
                throw new ArgumentException("Данные о самолёте недоступны");

            var allSeats = flight.Airplane.Seats;
            var occupiedSeatIds = (await _repository.GetAllAsync())
                .Where(t => t.FlightId == flightId && t.BookingStatus is "Зарезервирован" or "Оплачен")
                .Select(t => t.SeatId)
                .ToHashSet();

            return allSeats.Where(s => !occupiedSeatIds.Contains(s.Id)).ToList();
        }

        public async Task<IEnumerable<Seat>> GetOccupiedSeatsAsync(int flightId)
        {
            var flight = await _flightRepository.GetByIdWithAirplaneAndSeatsAsync(flightId);
            if (flight == null)
                throw new ArgumentException("Рейс не найден");
            
            if (flight.Airplane == null)
                throw new ArgumentException("Данные о самолёте недоступны");

            var allSeats = flight.Airplane.Seats;
            var occupiedSeatIds = (await _repository.GetAllAsync())
                .Where(t => t.FlightId == flightId && t.BookingStatus is "Зарезервирован" or "Оплачен")
                .Select(t => t.SeatId)
                .ToHashSet();

            return allSeats.Where(s => occupiedSeatIds.Contains(s.Id)).ToList();
        }

        public async Task CancelExpiredReservationsAsync()
        {
            await Task.CompletedTask;
        }
    }
}
