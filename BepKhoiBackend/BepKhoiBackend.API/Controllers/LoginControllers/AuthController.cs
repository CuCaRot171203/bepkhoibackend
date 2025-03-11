using Microsoft.AspNetCore.Mvc;
using BepKhoiBackend.BusinessObject.DTOs;
using BepKhoiBackend.BusinessObject.Interfaces;
using BepKhoiBackend.BusinessObject.dtos.LoginDto;
using Microsoft.AspNetCore.Http;


namespace BepKhoiBackend.API.Controllers.LoginControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthController(IAuthService authService, IHttpContextAccessor httpContextAccessor)
        {
            _authService = authService;
            _httpContextAccessor = httpContextAccessor;
        }
       

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequestDto loginRequest)
        {
            if (loginRequest == null || string.IsNullOrEmpty(loginRequest.Email) || string.IsNullOrEmpty(loginRequest.Password))
            {
                return BadRequest(new { message = "Email and Password are required" });
            }

            var user = _authService.ValidateUser(loginRequest);
            if (user == null)
            {
                return Unauthorized(new { message = "fail" });
            }
           
            if (user.IsVerify == false)
            {
                return Ok(new { message = "not_verify"});
            }

            var token = _authService.GenerateJwtToken(user);
            // Lưu thông tin vào Session
            var session = _httpContextAccessor.HttpContext.Session;
            session.SetString("Token", token);
            session.SetString("UserId", user.UserId.ToString());
            session.SetString("Phone", user.Email);
            return Ok(new { message = "succesfull", token , userId = user.UserId});
        }


    }
}
