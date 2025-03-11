using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BepKhoiBackend.BusinessObject.dtos.LoginDto;
using BepKhoiBackend.BusinessObject.DTOs;
using BepKhoiBackend.BusinessObject.Interfaces;
using BepKhoiBackend.BusinessObject.Services.LoginService;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IOtpService _otpService;
    private readonly IConfiguration _configuration;

    public UserService(IUserRepository userRepository, IOtpService otpService, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _otpService = otpService;
        _configuration = configuration;
    }
    public UserDto GetUserByEmail(string email)
    {
        var user = _userRepository.GetUserByEmail(email);
        if (user == null)
        {
            return null; // Trả về null nếu không tìm thấy user
        }

        return new UserDto
        {
            UserId = user.UserId,
            Email = user.Email,
            IsVerify = user.IsVerify
        };
    }

    public async Task<bool> VerifyUserByEmail(string email, string otp)
    {
        if (!_otpService.VerifyOtp(email, otp))
            return false;

        var user = _userRepository.GetUserByEmail(email);
        if (user == null)
            return false;

        user.IsVerify = true;
        _userRepository.UpdateUser(user);
        return true;
    }

    public string GenerateJwtToken(UserDto user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        string secretKey = _configuration["Jwt:Key"];

        if (string.IsNullOrEmpty(secretKey))
        {
            throw new ArgumentNullException("Jwt:Key", "JWT secret key is missing in configuration.");
        }

        var key = Encoding.UTF8.GetBytes(secretKey);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Email, user.Email)
        }),
            Expires = DateTime.UtcNow.AddHours(2),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    //code logic reset password
    public async Task<bool> ForgotPassword(ForgotPasswordDto request)
    {
        var user = _userRepository.GetUserByEmail(request.Email);
        if (user == null)
        {
            return false; // Người dùng không tồn tại
        }
        // Mã hóa mật khẩu mới
        user.Password = request.NewPassword;
        _userRepository.UpdateUser(user);
        return true;
    }

    //change password
    public async Task<string> ChangePassword(ChangePasswordDto request)
    {
        var user = _userRepository.GetUserByEmail(request.Email);
        if (user == null)
        {
            return "UserNotFound"; // Không tìm thấy tài khoản
        }

        // Kiểm tra mật khẩu cũ có đúng không (không dùng BCrypt)
        if (user.Password != request.OldPassword)
        {
            return "WrongPassword"; // Sai mật khẩu cũ
        }

        // Mã hóa mật khẩu mới và cập nhật vào database
        //user.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        user.Password = request.NewPassword;
        _userRepository.UpdateUser(user);

        return "Success"; // Đổi mật khẩu thành công
    }

}
