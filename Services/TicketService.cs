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
            // Проверяем существование рейса и загружаем данные о самолёте
            var flight = await _flightRepository.GetByIdWithAirplaneAsync(flightId);
            if (flight == null)
                throw new ArgumentException("Рейс не найден");
            
            if (flight.Airplane == null)
                throw new ArgumentException("Данные о самолёте недоступны");

            // Валидация формата номера места (например, "1A", "12B", "25C")
            if (!IsValidSeatNumber(seatNumber))
                throw new ArgumentException("Неверный формат номера места");

            // Проверяем, не занято ли место
            var existingTickets = await _repository.GetAllAsync();
            var seatTaken = existingTickets.Any(t => 
                t.FlightId == flightId && 
                t.SeatNumber.ToUpper() == seatNumber.ToUpper() && 
                t.BookingStatus is "Зарезервирован" or "Оплачен");
            
            if (seatTaken)
                throw new InvalidOperationException("Место уже занято");

            // Создаём резервирование
            var ticket = new Ticket
            {
                FlightId = flightId,
                PassengerId = passengerId,
                SeatNumber = seatNumber.ToUpper(),
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

        public async Task<bool> CancelPaidTicketAsync(int ticketId, int passengerId)
        {
            var ticket = await _repository.GetByIdAsync(ticketId);
            if (ticket == null || ticket.PassengerId != passengerId)
                return false;

            if (ticket.BookingStatus != "Оплачен")
                return false;

            ticket.BookingStatus = "Отменен";
            await _repository.UpdateAsync(ticket);

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

            var allTickets = await _repository.GetAllAsync();
            var occupiedSeats = allTickets
                .Where(t => t.FlightId == flightId && t.BookingStatus is "Зарезервирован" or "Оплачен")
                .Select(t => t.SeatNumber.ToUpper())
                .ToHashSet();

            // Генерируем все возможные места на основе вместимости самолёта
            var availableSeats = new List<string>();
            int rows = flight.Airplane.Capacity / 4; // 4 места в ряду (A, B, C, D)
            char[] seatLetters = { 'A', 'B', 'C', 'D' };

            for (int row = 1; row <= rows; row++)
            {
                foreach (var letter in seatLetters)
                {
                    string seatNumber = $"{row}{letter}";
                    if (!occupiedSeats.Contains(seatNumber))
                    {
                        availableSeats.Add(seatNumber);
                    }
                }
            }

            return availableSeats;
        }

        public async Task<IEnumerable<string>> GetOccupiedSeatsAsync(int flightId)
        {
            var allTickets = await _repository.GetAllAsync();
            return allTickets
                .Where(t => t.FlightId == flightId && t.BookingStatus is "Зарезервирован" or "Оплачен")
                .Select(t => t.SeatNumber.ToUpper())
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
    }

}
