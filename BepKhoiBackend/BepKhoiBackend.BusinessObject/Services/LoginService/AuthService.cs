using BepKhoiBackend.BusinessObject.DTOs;
using BepKhoiBackend.BusinessObject.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using BepKhoiBackend.BusinessObject.dtos.LoginDto;

namespace BepKhoiBackend.BusinessObject.Services.LoginService
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public AuthService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public UserDto? ValidateUser(LoginRequestDto loginRequest)
        {
            var user = _userRepository.GetUserByEmail(loginRequest.Email);
            if (user == null || user.Password != loginRequest.Password)
            {
                return null;
            }

            return new UserDto
            {
                UserId = user.UserId,
                Email = user.Email,
                IsVerify = user.IsVerify
            };
        }

        public string GenerateJwtToken(UserDto user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    new Claim(ClaimTypes.Name, user.Email)
                }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
