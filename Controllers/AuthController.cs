using AirlineAPI.Entity;
using AirlineAPI.Entity.Auth;
using AirlineAPI.Interfaces.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AirlineAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly IUserRepository _userRepository;

        public AuthController(AuthService authService, IUserRepository userRepository)
        {
            _authService = authService;
            _userRepository = userRepository;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            await _authService.RegisterAsync(request);
            return Ok("Регистрация прошла успешно");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var token = await _authService.LoginAsync(request);
            return Ok(new AuthResponse { Token = token });
        }
        
        /// <summary>
        /// Получить данные текущего авторизованного пользователя.
        /// </summary>
        [Authorize]
        [HttpGet("me")]
        public async Task<ActionResult<UserResponse>> GetCurrentUser()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var user = await _userRepository.GetByIdAsync(userId);
            
            if (user == null)
                return NotFound(new { message = "Пользователь не найден" });
            
            var response = new UserResponse
            {
                Id = user.Id,
                Username = user.Username,
                Role = user.Role,
                Passenger = user.Passenger != null ? new PassengerInfo
                {
                    Id = user.Passenger.Id,
                    FirstName = user.Passenger.FirstName,
                    LastName = user.Passenger.LastName,
                    Email = user.Passenger.Email,
                    Phone = user.Passenger.Phone
                } : null
            };
            
            return Ok(response);
        }
    }
}
