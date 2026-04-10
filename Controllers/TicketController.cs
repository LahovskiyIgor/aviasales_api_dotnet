using AirlineAPI.Entity;
using AirlineAPI.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AirlineAPI.Interfaces.Services;

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
        public async Task<ActionResult<IEnumerable<Ticket>>> GetAll() =>
            Ok(await _service.GetAllAsync());

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<Ticket>> GetById(int id)
        {
            var entity = await _service.GetByIdAsync(id);
            if (entity == null)
                return NotFound();
            else return Ok(entity);
        }

        /// <summary>
        /// Получить билет по ID. Доступно администратору или владельцу билета.
        /// </summary>
        [Authorize]
        [HttpGet("my/{id}")]
        public async Task<ActionResult<Ticket>> GetMyTicketById(int id)
        {
            var passengerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var ticket = await _service.GetByPassengerAndIdAsync(id, passengerId);
            
            if (ticket == null)
                return NotFound(new { message = "Билет не найден или не принадлежит вам" });
            
            return Ok(ticket);
        }

        /// <summary>
        /// Получить все билеты текущего пользователя.
        /// </summary>
        [Authorize]
        [HttpGet("my")]
        public async Task<ActionResult<IEnumerable<Ticket>>> GetMyTickets()
        {
            var passengerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var allTickets = await _service.GetAllWithDetailsAsync();
            var myTickets = allTickets.Where(t => t.PassengerId == passengerId);
            
            return Ok(myTickets);
        }

        /// <summary>
        /// Получить занятые места на рейсе.
        /// </summary>
        [Authorize]
        [HttpGet("flight/{flightId}/occupied-seats")]
        public async Task<ActionResult<IEnumerable<Seat>>> GetOccupiedSeats(int flightId)
        {
            try
            {
                var seats = await _service.GetOccupiedSeatsAsync(flightId);
                return Ok(seats);
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
        public async Task<ActionResult<IEnumerable<Seat>>> GetAvailableSeats(int flightId)
        {
            try
            {
                var seats = await _service.GetAvailableSeatsAsync(flightId);
                return Ok(seats);
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
                return CreatedAtAction(nameof(GetById), new { id = ticket.Id }, ticket);
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
            return CreatedAtAction(nameof(GetById), new { id = ticket.Id }, ticket);
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
