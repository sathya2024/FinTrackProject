using Microsoft.AspNetCore.Mvc;
using PortfolioTrackerApi.Models;
using PortfolioTrackerApi.Services;
using System;
using System.Threading.Tasks;
 
namespace PortfolioTrackerApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailVerificationController : ControllerBase
    {
        private readonly IEmailVerificationService _emailVerificationService;
 
        public EmailVerificationController(IEmailVerificationService emailVerificationService)
        {
            _emailVerificationService = emailVerificationService ?? throw new ArgumentNullException(nameof(emailVerificationService));
        }
 
        [HttpPost("send")]
        public async Task<IActionResult> SendVerificationCode([FromBody] EmailRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Email))
                return BadRequest("Email address is required.");
 
            try
            {
                await _emailVerificationService.SendVerificationCode(request.Email);
                return Ok(new { success = true, message = $"Verification code sent to {request.Email}" });
 
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while sending the verification email. Details: {ex.Message}");
            }
        }
 
        [HttpPost("verify")]
        public IActionResult VerifyCode([FromBody] VerificationRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Code))
                return BadRequest(new { success = false, message = "Email and verification code are required." });
 
            try
            {
                bool isVerified = _emailVerificationService.VerifyCode(request.Email, request.Code);
                if (isVerified)
                    return Ok(new { success = true, message = "Email verified successfully." });
                else
                    return BadRequest(new { success = false, message = "Invalid verification code or email." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"An error occurred during verification. Details: {ex.Message}" });
            }
        }
 
    }
}