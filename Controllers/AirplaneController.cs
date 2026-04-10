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
    public class AirplaneController : ControllerBase
    {
        private readonly IAirplaneService _service;

        public AirplaneController(IAirplaneService service)
        {
            _service = service;
        }

        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AirplaneDto>>> GetAll()
        {
            var airplanes = await _service.GetAllAsync();
            return Ok(airplanes.Select(a => a.ToDto()));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AirplaneDto>> GetById(int id)
        {
            var entity = await _service.GetByIdAsync(id);
            if (entity == null) 
                return NotFound(); 
            return Ok(entity.ToDto());
        }

        [HttpGet("{id}/flights")]
        public async Task<IActionResult> GetAirplaneWithFlights(int id)
        {
            var airplane = await _service.GetAirplaneWithFlightsAsync(id);
            if (airplane == null)
                return NotFound($"Самолёт с ID {id} не найден");

            return Ok(airplane.ToWithFlightsDto());
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Airplane airplane)
        {
            await _service.AddWithSeatsAsync(airplane);
            return CreatedAtAction(nameof(GetById), new { id = airplane.Id }, airplane.ToDto());
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Airplane airplane)
        {
            var existing = await _service.GetByIdAsync(id);
            if (existing == null)
                return NotFound();

            airplane.Id = id;
            await _service.UpdateAsync(airplane);
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _service.GetByIdAsync(id);
            if (existing == null)
                return NotFound();

            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}
