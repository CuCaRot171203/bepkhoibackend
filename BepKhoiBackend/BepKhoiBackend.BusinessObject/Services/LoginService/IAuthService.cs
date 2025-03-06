using BepKhoiBackend.BusinessObject.dtos.LoginDto;
using BepKhoiBackend.BusinessObject.DTOs;

namespace BepKhoiBackend.BusinessObject.Interfaces
{
    public interface IAuthService
    {
        UserDto? ValidateUser(LoginRequestDto loginRequest);
        string GenerateJwtToken(UserDto user);
    }
}
