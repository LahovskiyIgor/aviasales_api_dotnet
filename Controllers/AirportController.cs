using AirlineAPI.DTOs;
using AirlineAPI.Entity;
using AirlineAPI.Mappers;
using AirlineAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        public async Task<IActionResult> Create([FromBody] CreateAirportDto dto)
        {
            var airport = new AirportEntity
            {
                Name = dto.Name,
                Location = dto.Location
            };
            await _service.AddAsync(airport);
            return CreatedAtAction(nameof(GetById), new { id = airport.Id }, airport.ToDto());
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateAirportDto dto)
        {
            var existing = await _service.GetByIdAsync(id);
            if (existing == null) return NotFound();

            existing.Name = dto.Name;
            existing.Location = dto.Location;
            
            await _service.UpdateAsync(existing);
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _service.DeleteAsync(id);
                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(409, "Невозможно удалить аэропорт: он используется в рейсах.");
            }
        }
    }
}
