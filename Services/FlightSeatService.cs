using AirlineAPI.Entity;
using AirlineAPI.Interfaces.Repositories;

namespace AirlineAPI.Services
{
    public class FlightSeatService : IFlightSeatService
    {
        private readonly IFlightSeatRepository _seatRepository;
        
        public FlightSeatService(IFlightSeatRepository seatRepository)
        {
            _seatRepository = seatRepository;
        }

        public async Task<IEnumerable<FlightSeat>> GetAllSeatsAsync(int flightId) =>
            await _seatRepository.GetByFlightIdAsync(flightId);

        public async Task<FlightSeat?> GetSeatAsync(int flightId, string seatNumber) =>
            await _seatRepository.GetByFlightAndSeatNumberAsync(flightId, seatNumber);

        public async Task InitializeSeatsForFlightAsync(int flightId, int capacity) =>
            await _seatRepository.InitializeSeatsForFlightAsync(flightId, capacity);

        public async Task<bool> ReserveSeatAsync(int flightId, string seatNumber, int ticketId)
        {
            var seat = await _seatRepository.GetByFlightAndSeatNumberAsync(flightId, seatNumber);
            if (seat == null)
                return false;

            if (seat.Status != SeatStatus.Available)
                return false;

            seat.Status = SeatStatus.Reserved;
            seat.TicketId = ticketId;
            
            await _seatRepository.UpdateAsync(seat);
            return true;
        }

        public async Task<bool> ReleaseSeatAsync(int flightId, string seatNumber)
        {
            var seat = await _seatRepository.GetByFlightAndSeatNumberAsync(flightId, seatNumber);
            if (seat == null)
                return false;

            seat.Status = SeatStatus.Available;
            seat.TicketId = null;
            
            await _seatRepository.UpdateAsync(seat);
            return true;
        }

        public async Task UpdateSeatStatusFromTicketAsync(int flightId, string seatNumber, string bookingStatus)
        {
            var seat = await _seatRepository.GetByFlightAndSeatNumberAsync(flightId, seatNumber);
            if (seat == null)
                return;

            seat.Status = bookingStatus switch
            {
                "Зарезервирован" => SeatStatus.Reserved,
                "Оплачен" => SeatStatus.Sold,
                "Отменен" => SeatStatus.Available,
                _ => seat.Status
            };

            // Если билет отменён, сбрасываем связь с билетом
            if (bookingStatus == "Отменен")
            {
                seat.TicketId = null;
            }

            await _seatRepository.UpdateAsync(seat);
        }
    }
}
