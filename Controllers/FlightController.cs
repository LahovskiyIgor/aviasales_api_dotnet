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
using Microsoft.AspNetCore.Authorization;

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
        public async Task<ActionResult<IEnumerable<Flight>>> GetAll() =>
            Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<ActionResult<Flight>> GetById(int id)
        {
            var entity = await _service.GetByIdAsync(id);
            if (entity == null)
                return NotFound();
            else return Ok(entity);
        }

        [HttpGet("{id}/details")]
        public async Task<IActionResult> GetFlightDetails(int id)
        {
            var flight = await _service.GetFlightWithDetailsAsync(id);
            if (flight == null)
                return NotFound();

            return Ok(flight);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Flight flight)
        {
            await _service.AddAsync(flight);
            return CreatedAtAction(nameof(GetById), new { id = flight.Id }, flight);
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
