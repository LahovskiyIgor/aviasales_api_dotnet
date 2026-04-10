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
        public async Task<IActionResult> Create([FromBody] CreateFlightDto dto)
        {
            var flight = new Flight
            {
                FlightNumber = dto.FlightNumber,
                DepartureAirportId = dto.DepartureAirportId,
                ArrivalAirportId = dto.ArrivalAirportId,
                DepartureTime = dto.DepartureTime,
                ArrivalTime = dto.ArrivalTime,
                AirplaneId = dto.AirplaneId,
                TotalSeats = dto.TotalSeats,
                BasePrice = dto.BasePrice
            };
            await _service.AddAsync(flight);
            return CreatedAtAction(nameof(GetById), new { id = flight.Id }, flight.ToDto());
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateFlightDto dto)
        {
            var existing = await _service.GetByIdAsync(id);
            if (existing == null) return NotFound();

            existing.FlightNumber = dto.FlightNumber;
            existing.DepartureAirportId = dto.DepartureAirportId;
            existing.ArrivalAirportId = dto.ArrivalAirportId;
            existing.DepartureTime = dto.DepartureTime;
            existing.ArrivalTime = dto.ArrivalTime;
            existing.AirplaneId = dto.AirplaneId;
            existing.TotalSeats = dto.TotalSeats;
            existing.BasePrice = dto.BasePrice;

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
                return StatusCode(409, "Невозможно удалить рейс: он имеет связанные билеты.");
            }
        }
    }
}
