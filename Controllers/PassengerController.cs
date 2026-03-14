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
    public class PassengerController : ControllerBase
    {
        private readonly IPassengerService _service;

        public PassengerController(IPassengerService service)
        {
            _service = service;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Passenger>>> GetAll() =>
            Ok(await _service.GetAllAsync());

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<Passenger>> GetById(int id)
        {
            var entity = await _service.GetByIdAsync(id);
            if (entity == null)
                return NotFound();
            else return Ok(entity);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}/details")]
        public async Task<IActionResult> GetPassengerWithTicketsAndFlights(int id)
        {
            var passenger = await _service.GetPassengerWithTicketsAndFlightsAsync(id);
            if (passenger == null)
                return NotFound();

            return Ok(passenger);
        }

        [Authorize(Roles = "Admin,User")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Passenger passenger)
        {
            await _service.AddAsync(passenger);
            return CreatedAtAction(nameof(GetById), new { id = passenger.Id }, passenger);
        }

        [Authorize(Roles = "Admin,User")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Passenger passenger)
        {
            if (id != passenger.Id) return BadRequest();
            await _service.UpdateAsync(passenger);
            return NoContent();
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
