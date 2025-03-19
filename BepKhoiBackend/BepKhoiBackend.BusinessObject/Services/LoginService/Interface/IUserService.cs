using BepKhoiBackend.BusinessObject.dtos.LoginDto;
using BepKhoiBackend.BusinessObject.DTOs;
using BepKhoiBackend.DataAccess.Models;

namespace BepKhoiBackend.BusinessObject.Services.LoginService.Interface
{

    public interface IUserService
    {
        public UserDto GetUserByEmail(string email);
        Task<bool> VerifyUserByEmail(string email, string otp);
        string GenerateJwtToken(UserDto user);
        Task<bool> ForgotPassword(ForgotPasswordDto request);
        Task<string> ChangePassword(ChangePasswordDto request);
        bool IsValidEmail(string email);
    }

}
