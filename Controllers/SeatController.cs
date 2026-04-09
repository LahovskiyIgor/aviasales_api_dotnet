using AirlineAPI.Entity;
using AirlineAPI.Interfaces.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace AirlineAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SeatController : ControllerBase
    {
        private readonly ISeatRepository _repository;

        public SeatController(ISeatRepository repository)
        {
            _repository = repository;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Seat>>> GetAll() =>
            Ok(await _repository.GetAllAsync());

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<Seat>> GetById(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
                return NotFound();
            return Ok(entity);
        }

        [Authorize]
        [HttpGet("airplane/{airplaneId}")]
        public async Task<ActionResult<IEnumerable<Seat>>> GetByAirplane(int airplaneId)
        {
            var seats = await _repository.GetByAirplaneIdAsync(airplaneId);
            return Ok(seats);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Seat seat)
        {
            await _repository.AddAsync(seat);
            return CreatedAtAction(nameof(GetById), new { id = seat.Id }, seat);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Seat seat)
        {
            if (id != seat.Id) return BadRequest();
            await _repository.UpdateAsync(seat);
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _repository.DeleteAsync(id);
            return NoContent();
        }
    }
}
