using AirlineAPI.Entity;
using AirlineAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;

using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace AirlineAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SeatController : ControllerBase
    {
        private readonly ISeatService _service;

        public SeatController(ISeatService service)
        {
            _service = service;
        }

        /// <summary>
        /// Получить все места рейса.
        /// </summary>
        [Authorize]
        [HttpGet("flight/{flightId}")]
        public async Task<ActionResult<IEnumerable<Seat>>> GetSeatsByFlight(int flightId)
        {
            try
            {
                var seats = await _service.GetByFlightIdAsync(flightId);
                return Ok(seats);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Инициализировать места для рейса (только Admin).
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPost("flight/{flightId}/initialize")]
        public async Task<IActionResult> InitializeSeats(int flightId)
        {
            try
            {
                await _service.InitializeSeatsForFlightAsync(flightId);
                return Ok(new { message = "Места успешно созданы" });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Обновить статус доступности места (только Admin).
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPut("{seatId}/status")]
        public async Task<IActionResult> UpdateSeatStatus(int seatId, [FromBody] UpdateSeatStatusRequest request)
        {
            try
            {
                await _service.UpdateSeatStatusAsync(seatId, request.IsAvailable);
                return Ok(new { message = "Статус места обновлён" });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Удалить место (только Admin).
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{seatId}")]
        public async Task<IActionResult> DeleteSeat(int seatId)
        {
            await _service.DeleteAsync(seatId);
            return NoContent();
        }
    }

    public class UpdateSeatStatusRequest
    {
        public bool IsAvailable { get; set; }
    }
}
