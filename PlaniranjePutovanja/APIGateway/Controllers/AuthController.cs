using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using PlaniranjePutovanja.Common.DTOs.Auth;
using PlaniranjePutovanja.Common.Interfaces.Auth;

namespace PlaniranjePutovanja.APIGateway.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            try
            {
                var result = await _authService.RegisterUserAsync(request);

                if (result.IsSuccess)
                {
                    return Ok(result);
                }

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Register failed.");
                return StatusCode(500, new { error = "The server could not complete the request right now." });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            try
            {
                var result = await _authService.LoginUserAsync(request);

                if (result.IsSuccess)
                {
                    return Ok(result);
                }

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login failed.");
                return StatusCode(500, new { error = "The server could not complete the request right now." });
            }
        }

        // Admin rute
        [HttpGet("admin/users")]
        [Authorize(Roles = "Admin")] 
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _authService.GetAllUsersAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get all users.");
                return StatusCode(500, new { error = "The server could not complete the request right now." });
            }
        }

        [HttpDelete("admin/users/{userId}")]
        [Authorize(Roles = "Admin")] 
        public async Task<IActionResult> DeleteUser(string userId)
        {
            try
            {
                var result = await _authService.DeleteUserAsync(userId);
                if (result)
                {
                    return NoContent();
                }
                return NotFound(new { error = "The requested user could not be found." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete user.");
                return StatusCode(500, new { error = "The server could not complete the request right now." });
            }
        }
    }
}
