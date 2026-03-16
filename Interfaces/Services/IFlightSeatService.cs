using AirlineAPI.Entity;

namespace AirlineAPI.Services
{
    public interface IFlightSeatService
    {
        Task<IEnumerable<FlightSeat>> GetAllSeatsAsync(int flightId);
        Task<FlightSeat?> GetSeatAsync(int flightId, string seatNumber);
        Task InitializeSeatsForFlightAsync(int flightId, int capacity);
        Task<bool> ReserveSeatAsync(int flightId, string seatNumber, int ticketId);
        Task<bool> ReleaseSeatAsync(int flightId, string seatNumber);
        Task UpdateSeatStatusFromTicketAsync(int flightId, string seatNumber, string bookingStatus);
    }
}
