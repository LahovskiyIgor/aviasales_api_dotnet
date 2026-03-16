using AirlineAPI.Entity;
using AirlineAPI.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirlineAPI.Services
{
    public class SeatService : ISeatService
    {
        private readonly ISeatRepository _repository;
        private readonly IFlightRepository _flightRepository;

        public SeatService(ISeatRepository repository, IFlightRepository flightRepository)
        {
            _repository = repository;
            _flightRepository = flightRepository;
        }

        public Task<IEnumerable<Seat>> GetAllAsync() => _repository.GetAllAsync();
        
        public Task<Seat> GetByIdAsync(int id) => _repository.GetByIdAsync(id);
        
        public Task<IEnumerable<Seat>> GetByFlightIdAsync(int flightId) => 
            _repository.GetByFlightIdAsync(flightId);

        public async Task InitializeSeatsForFlightAsync(int flightId)
        {
            var flight = await _flightRepository.GetByIdWithAirplaneAsync(flightId);
            if (flight == null)
                throw new ArgumentException("Рейс не найден");
            
            if (flight.Airplane == null)
                throw new ArgumentException("Данные о самолёте недоступны");

            // Проверяем, уже ли созданы места для этого рейса
            var existingSeats = await _repository.GetByFlightIdAsync(flightId);
            if (existingSeats.Any())
                return; // Места уже созданы

            // Генерируем места на основе вместимости самолёта
            var seats = new List<Seat>();
            int rows = flight.Airplane.Capacity / 4; // 4 места в ряду (A, B, C, D)
            char[] seatLetters = { 'A', 'B', 'C', 'D' };

            for (int row = 1; row <= rows; row++)
            {
                foreach (var letter in seatLetters)
                {
                    string seatNumber = $"{row}{letter}";
                    
                    // Определяем класс места (первые 2 ряда - Business, остальные - Economy)
                    string seatClass = row <= 2 ? "Business" : "Economy";
                    
                    seats.Add(new Seat
                    {
                        FlightId = flightId,
                        SeatNumber = seatNumber,
                        Class = seatClass,
                        IsAvailable = true
                    });
                }
            }

            await _repository.AddRangeAsync(seats);
        }

        public async Task UpdateSeatStatusAsync(int seatId, bool isAvailable)
        {
            var seat = await _repository.GetByIdAsync(seatId);
            if (seat == null)
                throw new ArgumentException("Место не найдено");

            seat.IsAvailable = isAvailable;
            await _repository.UpdateAsync(seat);
        }

        public Task DeleteAsync(int id) => _repository.DeleteAsync(id);
    }
}
