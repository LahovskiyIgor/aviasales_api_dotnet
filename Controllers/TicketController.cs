using AirlineAPI.Entity;
using AirlineAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;


using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

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

        [Authorize(Roles = "Admin,User")]
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
            var success = await _service.UpdateStatusAsync(ticketId, "Отменен", passengerId);

            if (success)
                return Ok(new { message = "Билет успешно отменен" });
            return Forbid();
        }

        [Authorize(Roles = "Admin,User")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }


}
