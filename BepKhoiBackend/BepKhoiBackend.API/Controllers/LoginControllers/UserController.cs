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
        private readonly IEmailService _emailService;
        private readonly IOtpService _otpService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserController(IUserService userService, IEmailService emailService, IOtpService otpService, IHttpContextAccessor httpContextAccessor)
        {
            _userService = userService;
            _emailService = emailService;
            _otpService = otpService;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost("send-otp")]
        public async Task<IActionResult> SendOtp([FromBody] EmailDto request)
        {
            if (string.IsNullOrEmpty(request.Email))
                return BadRequest(new { message = "Email is required" });

            // Kiểm tra xem user có tồn tại trong database không
            var user = _userService.GetUserByEmail(request.Email);
            if (user == null)
                return BadRequest(new { message = "User with this email does not exist" });

            // Tạo OTP
            var otp = _otpService.GenerateOtp(request.Email);

            // Gửi OTP qua email
            await _emailService.SendEmailAsync(request.Email, "Your OTP Code", $"Your OTP is: {otp}");

            return Ok(new { message = "OTP sent successfully" });
        }


        [HttpPost("verify")]
        public async Task<IActionResult> VerifyUser([FromBody] VerifyOtpDto request)
        {
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Otp))
                return BadRequest(new { message = "Email and OTP are required" });

            bool isVerified = await _userService.VerifyUserByEmail(request.Email, request.Otp);
            if (!isVerified)
                return BadRequest(new { message = "Invalid OTP or Email" });

            var user = _userService.GetUserByEmail(request.Email);
            var token = _userService.GenerateJwtToken(user);
            // Lưu thông tin vào Session
            var session = _httpContextAccessor.HttpContext.Session;
            session.SetString("Token", token);
            session.SetString("UserId", user.UserId.ToString());
            session.SetString("Phone", user.Email);

            return Ok(new { message = "Verification successful!", token, UserId = user.UserId });
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto request)
        {
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.NewPassword))
            {
                return BadRequest(new { message = "Enail not null!" });
            }

            var result = await _userService.ForgotPassword(request);
            if (!result)
            {
                return NotFound(new { message = "not found Email!" });
            }

            return Ok(new { message = "reset successfull!" });
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto request)
        {
            if (string.IsNullOrEmpty(request.Email) ||
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
                return NotFound(new { message = "Email number not found!" });
            }
            if (result == "WrongPassword")
            {
                return Unauthorized(new { message = "Old password is incorrect!" });
            }

            return Ok(new { message = "Password changed successfully!" });
        }
    }
}
