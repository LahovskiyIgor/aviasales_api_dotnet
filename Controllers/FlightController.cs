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
        private readonly IFlightSeatService _seatService;

        public FlightController(IFlightService service, IFlightSeatService seatService)
        {
            _service = service;
            _seatService = seatService;
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

        /// <summary>
        /// Получить все места рейса с их статусами.
        /// </summary>
        [Authorize]
        [HttpGet("{id}/seats")]
        public async Task<IActionResult> GetFlightSeats(int id)
        {
            var seats = await _seatService.GetAllSeatsAsync(id);
            
            // Если места ещё не инициализированы, нужно сначала получить данные о рейсе
            if (!seats.Any())
            {
                var flight = await _service.GetByIdAsync(id);
                if (flight == null)
                    return NotFound(new { message = "Рейс не найден" });
                
                // Инициализируем места
                await _seatService.InitializeSeatsForFlightAsync(id, flight.TotalSeats);
                seats = await _seatService.GetAllSeatsAsync(id);
            }
            
            return Ok(seats);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Flight flight)
        {
            await _service.AddAsync(flight);
            
            // Инициализируем места для нового рейса
            if (flight.Id > 0)
            {
                await _seatService.InitializeSeatsForFlightAsync(flight.Id, flight.TotalSeats);
            }
            
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
