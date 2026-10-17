using API_Aggregation.Interfaces;
using API_Aggregation.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_Aggregation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IConfiguration _configuration;

        public AuthController(IAuthService authService, IConfiguration configuration)
        {
            _authService = authService;
            _configuration = configuration;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var username = _configuration["TestUser:Username"];
            var password = _configuration["TestUser:Password"];

            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password))
            {
                return StatusCode(500, "Test user is not configured.");
            }

            if (request.Username != username || request.Password != password)
            {
                return Unauthorized(new { message = "Invalid username or password." });
            }

            var token = _authService.GenerateToken(request.Username);

            return Ok(new { token });
        }
    }
}
