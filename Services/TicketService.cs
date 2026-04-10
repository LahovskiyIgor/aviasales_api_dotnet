using AirlineAPI.Interfaces.Services;
using AirlineAPI.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AirlineAPI.Data;
using AirlineAPI.Entity;

namespace AirlineAPI.Services
{
    public class TicketService : ITicketService
    {
        private readonly ITicketRepository _repository;
        private readonly IFlightRepository _flightRepository;
        private readonly ISeatRepository _seatRepository;
        private readonly AppDbContext _context;
        private readonly ILogger<TicketService> _logger;

        public TicketService(
            ITicketRepository repository, 
            IFlightRepository flightRepository, 
            ISeatRepository seatRepository, 
            AppDbContext context,
            ILogger<TicketService> logger)
        {
            _repository = repository;
            _flightRepository = flightRepository;
            _seatRepository = seatRepository;
            _context = context;
            _logger = logger;
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
                BookingStatus = "Зарезервирован",
                ReservedAt = DateTime.UtcNow
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
            var expiredTickets = await _context.Tickets
                .Include(t => t.Flight)
                .Where(t => t.BookingStatus == "Зарезервирован")
                .ToListAsync();

            var now = DateTime.UtcNow;
            var canceledCount = 0;
            
            foreach (var ticket in expiredTickets)
            {
                // Отменяем резервирование если прошло больше 10 минут с момента бронирования
                // ИЛИ если вылет рейса меньше чем через 30 минут
                bool isTimeExpired = ticket.ReservedAt.HasValue && 
                                    (now - ticket.ReservedAt.Value).TotalMinutes > 10;
                bool isFlightSoon = ticket.Flight != null && 
                                   ticket.Flight.DepartureTime < now.AddMinutes(30);
                
                if (isTimeExpired || isFlightSoon)
                {
                    ticket.BookingStatus = "Отменен";
                    
                    if (ticket.Flight != null && ticket.Flight.ReservedTickets > 0)
                    {
                        ticket.Flight.ReservedTickets--;
                    }
                    
                    canceledCount++;
                }
            }
            
            await _context.SaveChangesAsync();
            
            if (canceledCount > 0)
            {
                _logger.LogInformation($"Отменено {canceledCount} просроченных резервирований");
            }
        }

        /// <summary>
        /// Получить оставшееся время бронирования в секундах для конкретного билета.
        /// Возвращает null если билет не зарезервирован или уже истёк.
        /// </summary>
        public async Task<int?> GetRemainingReservationTimeAsync(int ticketId, int passengerId)
        {
            var ticket = await _repository.GetByIdAsync(ticketId);
            if (ticket == null || ticket.PassengerId != passengerId)
                return null;

            if (ticket.BookingStatus != "Зарезервирован" || !ticket.ReservedAt.HasValue)
                return null;

            var now = DateTime.UtcNow;
            var elapsed = (now - ticket.ReservedAt.Value).TotalSeconds;
            var remaining = 600 - elapsed; // 10 минут = 600 секунд

            if (remaining <= 0)
                return 0;

            // Также проверяем время до вылета
            if (ticket.Flight != null)
            {
                var timeToDeparture = (ticket.Flight.DepartureTime - now).TotalMinutes;
                if (timeToDeparture < 30)
                {
                    // Если до вылета меньше 30 минут, резервирование скоро будет отменено
                    return Math.Max(0, (int)(timeToDeparture * 60));
                }
            }

            return (int)remaining;
        }
    }
}
