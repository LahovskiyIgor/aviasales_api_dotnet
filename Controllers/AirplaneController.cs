using AirlineAPI.Entity;
using AirlineAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.AspNetCore.Authorization;

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
        public async Task<ActionResult<IEnumerable<Airplane>>> GetAll() =>
            Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<ActionResult<Airplane>> GetById(int id)
        {
            var entity = await _service.GetByIdAsync(id);
            if (entity == null) 
                return NotFound(); 
            else return Ok(entity);
        }

        [HttpGet("{id}/flights")]
        public async Task<IActionResult> GetAirplaneWithFlights(int id)
        {
            var airplane = await _service.GetAirplaneWithFlightsAsync(id);
            if (airplane == null)
                return NotFound($"Самолёт с ID {id} не найден");

            return Ok(airplane);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Airplane airplane)
        {
            await _service.AddAsync(airplane);
            return CreatedAtAction(nameof(GetById), new { id = airplane.Id }, airplane);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Airplane airplane)
        {
            var existing = await _service.GetByIdAsync(id);
            if (existing == null)
                return NotFound();

            airplane.Id = id; // Убедимся, что ID не меняется
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
