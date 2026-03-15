using AirlineAPI.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirlineAPI.Services
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
        
        Task<Ticket> ReserveTicketAsync(int flightId, int passengerId, string seatNumber);
        Task<bool> CancelReservationAsync(int ticketId, int passengerId);
        Task<bool> CancelPaidTicketAsync(int ticketId, int passengerId);
        Task<IEnumerable<string>> GetAvailableSeatsAsync(int flightId);
        Task<IEnumerable<string>> GetOccupiedSeatsAsync(int flightId);
        Task CancelExpiredReservationsAsync();
    }

}
