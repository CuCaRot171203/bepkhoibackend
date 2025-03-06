using Microsoft.AspNetCore.Mvc;
using BepKhoiBackend.BusinessObject.DTOs;
using BepKhoiBackend.BusinessObject.Interfaces;
using BepKhoiBackend.BusinessObject.dtos.LoginDto;

namespace BepKhoiBackend.API.Controllers.LoginControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequestDto loginRequest)
        {
            if (loginRequest == null || string.IsNullOrEmpty(loginRequest.Phone) || string.IsNullOrEmpty(loginRequest.Password))
            {
                return BadRequest(new { message = "Phone and Password are required" });
            }

            var user = _authService.ValidateUser(loginRequest);
            if (user == null)
            {
                return Unauthorized(new { message = "fail" });
            }

            if (user.IsVerify == false)
            {
                return Ok(new { message = "not_verify", token = _authService.GenerateJwtToken(user) });
            }

            var token = _authService.GenerateJwtToken(user);
            return Ok(new { message = "succesfull", token });
        }
    }
}
