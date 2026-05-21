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
        private readonly ILogger<AuthController> _logger;

        public AuthController(ILogger<AuthController> logger)
        {
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            try
            {
                var proxy = ServiceProxy.Create<IAuthService>(
                    new Uri("fabric:/PlaniranjePutovanja/PlaniranjePutovanja.AuthService"));

                var result = await proxy.RegisterUserAsync(request);

                if (result.IsSuccess)
                {
                    return Ok(result);
                }

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Register failed.");
                return StatusCode(500, $"Error: {ex.Message}, {ex.ToString()}");
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            try
            {
                var proxy = ServiceProxy.Create<IAuthService>(
                    new Uri("fabric:/PlaniranjePutovanja/PlaniranjePutovanja.AuthService"));

                var result = await proxy.LoginUserAsync(request);

                if (result.IsSuccess)
                {
                    return Ok(result);
                }

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login failed.");
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }
    }
}
