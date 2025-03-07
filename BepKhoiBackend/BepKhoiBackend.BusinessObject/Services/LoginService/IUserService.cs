using BepKhoiBackend.BusinessObject.dtos.LoginDto;
using BepKhoiBackend.BusinessObject.DTOs;
using BepKhoiBackend.DataAccess.Models;

namespace BepKhoiBackend.BusinessObject.Interfaces
{
    public interface IUserService
    {
        bool VerifyUser(VerifyUserDto request);
        Task<bool> ForgotPassword(ForgotPasswordDto request);
        Task<string> ChangePassword(ChangePasswordDto request);

    }
}
