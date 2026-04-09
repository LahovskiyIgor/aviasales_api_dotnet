using AirlineAPI.Entity;

namespace AirlineAPI.Interfaces.Services
{
    public interface ITicketService
    {
        Task<IEnumerable<Ticket>> GetAllAsync();
        Task<Ticket> GetByIdAsync(int id);
        Task AddAsync(Ticket ticket);
        Task UpdateAsync(Ticket ticket);
        Task DeleteAsync(int id);

        Task<bool> UpdateStatusAsync(int ticketId, string status, int passengerId);
        Task<Ticket> GetByPassengerAndIdAsync(int ticketId, int passengerId);
        
        Task<Ticket> ReserveTicketAsync(int flightId, int passengerId, int seatId);
        Task<bool> CancelReservationAsync(int ticketId, int passengerId);
        Task<bool> CancelPaidTicketAsync(int ticketId, int passengerId);
        Task<IEnumerable<Seat>> GetAvailableSeatsAsync(int flightId);
        Task<IEnumerable<Seat>> GetOccupiedSeatsAsync(int flightId);
        Task CancelExpiredReservationsAsync();
    }
}
