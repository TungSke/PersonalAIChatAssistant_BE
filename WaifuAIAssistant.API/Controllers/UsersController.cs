using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using WaifuAIAssistant.Application.DTOs.Request;
using WaifuAIAssistant.Application.Interfaces;
using WaifuAIAssistant.Service.DTOs.Request;

namespace WaifuAIAssistant.API.Controllers
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
        public async Task<IActionResult> RefreshToken(RefreshTokenRequest request)
        {
            var response = await _userService.RefreshToken(request);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
    }
}
