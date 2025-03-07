using BepKhoiBackend.BusinessObject.dtos.LoginDto;
using BepKhoiBackend.BusinessObject.Interfaces;
using BepKhoiBackend.BusinessObject.Services;
using BepKhoiBackend.BusinessObject.Services.LoginService;
using BepKhoiBackend.DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using BepKhoiBackend.BusinessObject.DTOs;

namespace BepKhoiBackend.API.Controllers.LoginControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IAuthService _authService;

        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserController(IUserService userService, IHttpContextAccessor httpContextAccessor)
        {
            _userService = userService;
            _httpContextAccessor = httpContextAccessor;
        }
       
        [HttpPost("verify")]
        public IActionResult VerifyUser([FromBody] VerifyUserDto request)
        {
            if (string.IsNullOrEmpty(request.PhoneNumber))
                return BadRequest(new { message = "phone not null" });

            bool isVerified = _userService.VerifyUser(request);
            if (!isVerified)
                return NotFound(new { message = "not found user!" });
            return Ok(new { message = "successful!", verify = true });
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto request)
        {
            if (string.IsNullOrEmpty(request.PhoneNumber) || string.IsNullOrEmpty(request.NewPassword))
            {
                return BadRequest(new { message = "phone not null!" });
            }

            var result = await _userService.ForgotPassword(request);
            if (!result)
            {
                return NotFound(new { message = "not found phone!" });
            }

            return Ok(new { message = "reset successfull!" });
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto request)
        {
            if (string.IsNullOrEmpty(request.PhoneNumber) ||
                string.IsNullOrEmpty(request.OldPassword) ||
                string.IsNullOrEmpty(request.NewPassword) ||
                string.IsNullOrEmpty(request.RePassword))
            {
                return BadRequest(new { message = "All fields are required!" });
            }

            if (request.NewPassword != request.RePassword)
            {
                return BadRequest(new { message = "New passwords do not match!" });
            }

            var result = await _userService.ChangePassword(request);
            if (result == "UserNotFound")
            {
                return NotFound(new { message = "Phone number not found!" });
            }
            if (result == "WrongPassword")
            {
                return Unauthorized(new { message = "Old password is incorrect!" });
            }

            return Ok(new { message = "Password changed successfully!" });
        }
    }
}
