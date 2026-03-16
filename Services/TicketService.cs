using AirlineAPI.Entity;
using AirlineAPI.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace AirlineAPI.Services
{
    public class TicketService : ITicketService
    {
        private readonly ITicketRepository _repository;
        private readonly IFlightRepository _flightRepository;
        private readonly IFlightSeatRepository _seatRepository;

        public TicketService(ITicketRepository repository, IFlightRepository flightRepository, IFlightSeatRepository seatRepository)
        {
            _repository = repository;
            _flightRepository = flightRepository;
            _seatRepository = seatRepository;
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
            // Проверяем существование рейса и загружаем данные о самолёте
            var flight = await _flightRepository.GetByIdWithAirplaneAsync(flightId);
            if (flight == null)
                throw new ArgumentException("Рейс не найден");
            
            if (flight.Airplane == null)
                throw new ArgumentException("Данные о самолёте недоступны");

            // Инициализируем места для рейса, если они еще не созданы
            await InitializeFlightSeatsAsync(flightId);

            // Валидация формата номера места (например, "1A", "12B", "25C")
            if (!IsValidSeatNumber(seatNumber))
                throw new ArgumentException("Неверный формат номера места");

            // Проверяем существование места в базе данных
            var flightSeat = await _seatRepository.GetByFlightAndSeatNumberAsync(flightId, seatNumber.ToUpper());
            if (flightSeat == null)
                throw new ArgumentException("Место не найдено на данном рейсе");

            // Проверяем, не занято ли место
            if (!flightSeat.IsAvailable)
                throw new InvalidOperationException("Место уже занято");

            // Создаём резервирование
            var ticket = new Ticket
            {
                FlightId = flightId,
                PassengerId = passengerId,
                SeatNumber = seatNumber.ToUpper(),
                BookingStatus = "Зарезервирован",
                FlightSeatId = flightSeat.Id
            };

            await _repository.AddAsync(ticket);
            
            // Обновляем статус места
            flightSeat.IsAvailable = false;
            flightSeat.TicketId = ticket.Id;
            await _seatRepository.UpdateAsync(flightSeat);
            
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

            // Освобождаем место
            if (ticket.FlightSeatId.HasValue)
            {
                var flightSeat = await _seatRepository.GetByIdAsync(ticket.FlightSeatId.Value);
                if (flightSeat != null)
                {
                    flightSeat.IsAvailable = true;
                    flightSeat.TicketId = null;
                    await _seatRepository.UpdateAsync(flightSeat);
                }
            }

            // Обновляем счётчик зарезервированных мест
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

            // Освобождаем место
            if (ticket.FlightSeatId.HasValue)
            {
                var flightSeat = await _seatRepository.GetByIdAsync(ticket.FlightSeatId.Value);
                if (flightSeat != null)
                {
                    flightSeat.IsAvailable = true;
                    flightSeat.TicketId = null;
                    await _seatRepository.UpdateAsync(flightSeat);
                }
            }

            // Обновляем счётчик проданных мест
            var flight = await _flightRepository.GetByIdAsync(ticket.FlightId);
            if (flight != null && flight.SoldTickets > 0)
            {
                flight.SoldTickets--;
                await _flightRepository.UpdateAsync(flight);
            }

            return true;
        }

        public async Task<IEnumerable<string>> GetAvailableSeatsAsync(int flightId)
        {
            var flight = await _flightRepository.GetByIdWithAirplaneAsync(flightId);
            if (flight == null)
                throw new ArgumentException("Рейс не найден");
            
            if (flight.Airplane == null)
                throw new ArgumentException("Данные о самолёте недоступны");

            // Получаем все места для рейса из базы данных
            var allSeats = await _seatRepository.GetByFlightIdAsync(flightId);
            
            // Если мест еще нет, генерируем их на основе вместимости
            if (!allSeats.Any())
            {
                await InitializeFlightSeatsAsync(flightId);
                allSeats = await _seatRepository.GetByFlightIdAsync(flightId);
            }

            // Возвращаем только доступные места
            return allSeats
                .Where(s => s.IsAvailable)
                .Select(s => s.SeatNumber)
                .ToList();
        }

        public async Task<IEnumerable<string>> GetOccupiedSeatsAsync(int flightId)
        {
            // Получаем все места для рейса из базы данных
            var allSeats = await _seatRepository.GetByFlightIdAsync(flightId);
            
            // Возвращаем только занятые места
            return allSeats
                .Where(s => !s.IsAvailable)
                .Select(s => s.SeatNumber)
                .ToList();
        }

        private bool IsValidSeatNumber(string seatNumber)
        {
            if (string.IsNullOrWhiteSpace(seatNumber))
                return false;

            // Формат: число + буква (например, "1A", "12B", "25C")
            var pattern = @"^[1-9][0-9]*[A-Da-d]$";
            return Regex.IsMatch(seatNumber, pattern);
        }

        public async Task CancelExpiredReservationsAsync()
        {
            // Метод устарел после удаления ReservationExpiresAt
            // Резервации теперь не имеют автоматического времени истечения
            await Task.CompletedTask;
        }

        public async Task InitializeFlightSeatsAsync(int flightId)
        {
            var flight = await _flightRepository.GetByIdWithAirplaneAsync(flightId);
            if (flight == null || flight.Airplane == null)
                return;

            await _seatRepository.InitializeSeatsForFlightAsync(flightId, flight.Airplane.Capacity);
        }
    }

}
