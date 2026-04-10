using AirlineAPI.DTOs;
using AirlineAPI.Mappers;
using AirlineAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AirlineAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AirportController : ControllerBase
    {
        private readonly IAirportService _service;

        public AirportController(IAirportService service)
        {
            _service = service;
        }

        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AirportDto>>> GetAll()
        {
            var airports = await _service.GetAllAsync();
            return Ok(airports.Select(a => a.ToDto()));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AirportDto>> GetById(int id)
        {
            var entity = await _service.GetByIdAsync(id);
            if (entity == null) return NotFound();
            return Ok(entity.ToDto());
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AirportEntity airport)
        {
            await _service.AddAsync(airport);
            return CreatedAtAction(nameof(GetById), new { id = airport.Id }, airport.ToDto());
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] AirportEntity airport)
        {
            if (id != airport.Id) return BadRequest();
            await _service.UpdateAsync(airport);
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
