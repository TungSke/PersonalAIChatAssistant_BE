using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.Authorization;
using PersonalAIAssistant.Application.DTOs.Request;
using PersonalAIAssistant.Service.DTOs.Request;
using PersonalAIAssistant.Application.Interfaces.Services;

namespace PersonalAIAssistant.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        [EndpointDescription("Register a new user")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            var response = await _userService.Register(request);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPost("login")]
        [EnableRateLimiting("loginPolicy")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var response = await _userService.Login(request);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);

        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> Me()
        {
            var response = await _userService.Me();
            if (response.Success)
            {
                return Ok(response);
            }

            return Unauthorized(response);
        }

        [HttpPost("verify-account")]
        public async Task<IActionResult> VerifyAccount(VerifyAccountRequest request)
        {
            var response = await _userService.VerifyAccount(request);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);

        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            var response = await _userService.RefreshToken();
            if (response.Success)
            {
                return Ok(response);
            }

            return Unauthorized(response);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var response = await _userService.Logout();
            if (response.Success)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }

        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin(string idToken)
        {
            var response = await _userService.GoogleLogin(idToken);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPost("register-with-phone-number")]
        public async Task<IActionResult> RegisterWithPhoneNumber(RegisterWithPhoneNumberRequest request)
        {
            var response = await _userService.RegisterWithPhoneNumber(request);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPost("verify-phone-number")]
        public async Task<IActionResult> VerifyPhoneNumber(VerifyPhoneNumberRequest request)
        {
            var response = await _userService.VerifyPhoneNumber(request);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
    }
}
