using AirlineAPI.DTOs;
using AirlineAPI.Entity;
using AirlineAPI.Mappers;
using AirlineAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AirlineAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketController : ControllerBase
    {
        private readonly ITicketService _service;

        public TicketController(ITicketService service)
        {
            _service = service;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TicketDto>>> GetAll()
        {
            var tickets = await _service.GetAllAsync();
            return Ok(tickets.Select(t => t.ToDto()));
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<TicketDto>> GetById(int id)
        {
            var entity = await _service.GetByIdAsync(id);
            if (entity == null)
                return NotFound();
            return Ok(entity.ToDto());
        }

        /// <summary>
        /// Получить билет по ID. Доступно администратору или владельцу билета.
        /// </summary>
        [Authorize]
        [HttpGet("my/{id}")]
        public async Task<ActionResult<TicketDto>> GetMyTicketById(int id)
        {
            var passengerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var ticket = await _service.GetByPassengerAndIdAsync(id, passengerId);
            
            if (ticket == null)
                return NotFound(new { message = "Билет не найден или не принадлежит вам" });
            
            return Ok(ticket.ToDto());
        }

        /// <summary>
        /// Получить все билеты текущего пользователя.
        /// </summary>
        [Authorize]
        [HttpGet("my")]
        public async Task<ActionResult<IEnumerable<TicketDto>>> GetMyTickets()
        {
            var passengerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var allTickets = await _service.GetAllWithDetailsAsync();
            var myTickets = allTickets.Where(t => t.PassengerId == passengerId);
            
            return Ok(myTickets.Select(t => t.ToDto()));
        }

        /// <summary>
        /// Получить занятые места на рейсе.
        /// </summary>
        [Authorize]
        [HttpGet("flight/{flightId}/occupied-seats")]
        public async Task<ActionResult<IEnumerable<SeatDto>>> GetOccupiedSeats(int flightId)
        {
            try
            {
                var seats = await _service.GetOccupiedSeatsAsync(flightId);
                return Ok(seats.Select(s => s.ToDto()));
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Получить доступные места на рейсе.
        /// </summary>
        [Authorize]
        [HttpGet("flight/{flightId}/available-seats")]
        public async Task<ActionResult<IEnumerable<SeatDto>>> GetAvailableSeats(int flightId)
        {
            try
            {
                var seats = await _service.GetAvailableSeatsAsync(flightId);
                return Ok(seats.Select(s => s.ToDto()));
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Admin,Passanger")]
        [HttpPost("reserve")]
        public async Task<IActionResult> ReserveTicket([FromBody] ReserveTicketRequest request)
        {
            try
            {
                var passengerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var ticket = await _service.ReserveTicketAsync(request.FlightId, passengerId, request.SeatId);
                return CreatedAtAction(nameof(GetById), new { id = ticket.Id }, ticket.ToDto());
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("{ticketId}/cancel-reservation")]
        public async Task<IActionResult> CancelReservation(int ticketId)
        {
            var passengerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var success = await _service.CancelReservationAsync(ticketId, passengerId);

            if (success)
                return Ok(new { message = "Резервирование успешно отменено" });
            return Forbid();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("cancel-expired")]
        public async Task<IActionResult> CancelExpiredReservations()
        {
            await _service.CancelExpiredReservationsAsync();
            return Ok(new { message = "Просроченные резервирования отменены" });
        }

        [Authorize(Roles = "Admin,Passanger")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Ticket ticket)
        {
            await _service.AddAsync(ticket);
            return CreatedAtAction(nameof(GetById), new { id = ticket.Id }, ticket.ToDto());
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Ticket ticket)
        {
            if (id != ticket.Id) return BadRequest();
            await _service.UpdateAsync(ticket);
            return NoContent();
        }

        [Authorize]
        [HttpPost("{ticketId}/pay")]
        public async Task<IActionResult> PayTicket(int ticketId)
        {
            var passengerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var success = await _service.UpdateStatusAsync(ticketId, "Оплачен", passengerId);

            if (success)
                return Ok(new { message = "Билет успешно оплачен" });
            return Forbid();
        }

        [Authorize]
        [HttpPost("{ticketId}/cancel")]
        public async Task<IActionResult> CancelTicket(int ticketId)
        {
            var passengerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            
            var success = await _service.CancelPaidTicketAsync(ticketId, passengerId);
            
            if (!success)
            {
                success = await _service.CancelReservationAsync(ticketId, passengerId);
            }

            if (success)
                return Ok(new { message = "Билет успешно отменен" });
            return Forbid();
        }

        [Authorize(Roles = "Admin,Passanger")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }


    public class ReserveTicketRequest
    {
        public int FlightId { get; set; }
        public int SeatId { get; set; }
    }
}
