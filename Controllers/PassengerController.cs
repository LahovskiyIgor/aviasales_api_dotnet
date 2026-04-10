using AirlineAPI.DTOs;
using AirlineAPI.Entity;
using AirlineAPI.Mappers;
using AirlineAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
        public async Task<ActionResult<IEnumerable<PassengerDto>>> GetAll()
        {
            var passengers = await _service.GetAllAsync();
            return Ok(passengers.Select(p => p.ToDto()));
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<PassengerDto>> GetById(int id)
        {
            var entity = await _service.GetByIdAsync(id);
            if (entity == null)
                return NotFound();
            return Ok(entity.ToDto());
        }

        /// <summary>
        /// Получить данные текущего пользователя (пассажира).
        /// </summary>
        [Authorize]
        [HttpGet("my")]
        public async Task<ActionResult<PassengerDto>> GetMyProfile()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            
            var passengers = await _service.GetAllAsync();
            var passenger = passengers.FirstOrDefault(p => p.UserId == userId);
            
            if (passenger == null)
                return NotFound(new { message = "Профиль пассажира не найден" });
            
            return Ok(passenger.ToDto());
        }

        /// <summary>
        /// Обновить данные текущего пользователя.
        /// </summary>
        [Authorize]
        [HttpPut("my")]
        public async Task<IActionResult> UpdateMyProfile([FromBody] UpdatePassengerDto passengerDto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            
            var existingPassengers = await _service.GetAllAsync();
            var existingPassenger = existingPassengers.FirstOrDefault(p => p.UserId == userId);
            
            if (existingPassenger == null)
                return NotFound(new { message = "Профиль пассажира не найден" });

            existingPassenger.FirstName = passengerDto.FirstName;
            existingPassenger.LastName = passengerDto.LastName;
            existingPassenger.Email = passengerDto.Email;
            existingPassenger.Phone = passengerDto.Phone;

            try
            {
                await _service.UpdateAsync(existingPassenger);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Ошибка при обновлении профиля: {ex.Message}" });
            }

        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}/details")]
        public async Task<IActionResult> GetPassengerWithTicketsAndFlights(int id)
        {
            var passenger = await _service.GetPassengerWithTicketsAndFlightsAsync(id);
            if (passenger == null)
                return NotFound();

            return Ok(passenger.ToDetailsDto());
        }

        [Authorize(Roles = "Admin,Passanger")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Passenger passenger)
        {
            await _service.AddAsync(passenger);
            return CreatedAtAction(nameof(GetById), new { id = passenger.Id }, passenger.ToDto());
        }

        [Authorize(Roles = "Admin,Passanger")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Passenger passenger)
        {
            if (id != passenger.Id) return BadRequest();
            await _service.UpdateAsync(passenger);
            return NoContent();
        }

        [Authorize(Roles = "Admin,Passanger")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}
