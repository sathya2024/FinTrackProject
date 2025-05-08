using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PortfolioTrackerApi.Models;
using PortfolioTrackerApi.Services;

namespace PortfolioTrackerApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IUserService userService, IJwtTokenService jwtTokenService, ILogger<AuthController> logger)
        {
            _userService = userService;
            _jwtTokenService = jwtTokenService;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            Console.WriteLine($"Login request: {request?.Email}, {request?.Password}");
            if (request == null || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest("Username or password not provided.");
            }

            try
            {
                var user = await _userService.ValidateCredentialsAsync(request.Email, request.Password);

                if (user == null)
                {
                    return Unauthorized("Invalid credentials.");
                }

                var token = _jwtTokenService.GenerateToken(user);

                return Ok(new
                {
                    Token = token,
                    User = new { user.UserName },
                    UserId = user.userId
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during login.");
                return StatusCode(500, "An internal server error occurred.");
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User newUser)
        {
            Console.WriteLine($"Register request: {newUser?.UserName}, {newUser?.Email}, {newUser?.Password}, {newUser?.ConfirmPassword}");
            var result = await _userService.RegisterUserAsync(newUser);
             if (!result)
    {
        return BadRequest(new { message = "Username or Email already exists." });
    }

    return Ok(new { message = "User registered successfully." });
        }
    }
}

