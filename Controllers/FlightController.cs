using AirlineAPI.DTOs;
using AirlineAPI.Entity;
using AirlineAPI.Mappers;
using AirlineAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AirlineAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FlightController : ControllerBase
    {
        private readonly IFlightService _service;

        public FlightController(IFlightService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<FlightDto>>> GetAll()
        {
            var flights = await _service.GetAllAsync();
            return Ok(flights.Select(f => f.ToDto()));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<FlightDto>> GetById(int id)
        {
            var entity = await _service.GetByIdAsync(id);
            if (entity == null)
                return NotFound();
            return Ok(entity.ToDto());
        }

        [HttpGet("{id}/details")]
        public async Task<IActionResult> GetFlightDetails(int id)
        {
            var flight = await _service.GetFlightWithDetailsAsync(id);
            if (flight == null)
                return NotFound();

            return Ok(flight.ToDetailsDto());
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Flight flight)
        {
            await _service.AddAsync(flight);
            return CreatedAtAction(nameof(GetById), new { id = flight.Id }, flight.ToDto());
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Flight flight)
        {
            if (id != flight.Id) return BadRequest();
            await _service.UpdateAsync(flight);
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}
