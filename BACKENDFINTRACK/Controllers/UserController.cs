using Microsoft.AspNetCore.Mvc;
using PortfolioTrackerApi.Models;
using PortfolioTrackerApi.Services;

namespace PortfolioTrackerApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User newUser)
        {
            if (string.IsNullOrWhiteSpace(newUser.UserName) || string.IsNullOrWhiteSpace(newUser.Password))
                return BadRequest("Username and password are required.");

            try
            {
                var success = await _userService.RegisterUserAsync(newUser);
                if (!success)
                    return Conflict("User already exists.");

                return Ok("Registration successful.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Registration failed.");
                return StatusCode(500, "Internal server error.");
            }
        }
    }
}
