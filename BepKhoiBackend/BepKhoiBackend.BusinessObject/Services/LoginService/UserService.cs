using BCrypt.Net;
using BepKhoiBackend.BusinessObject.dtos.LoginDto;
using BepKhoiBackend.BusinessObject.DTOs;
using BepKhoiBackend.BusinessObject.Interfaces;
using BepKhoiBackend.DataAccess.Models;
using BepKhoiBackend.DataAccess.Repository;
using BepKhoiBackend.DataAccess.Repository.LoginRepository;

namespace BepKhoiBackend.BusinessObject.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public bool VerifyUser(VerifyUserDto request)
        {
            var user = _userRepository.GetUserByPhone(request.PhoneNumber);
            if (user == null) return false;

            user.IsVerify = true;
            _userRepository.UpdateUser(user);
            return true;
        }

        //code logic reset password
        public async Task<bool> ForgotPassword(ForgotPasswordDto request)
        {
            var user =  _userRepository.GetUserByPhone(request.PhoneNumber);
            if (user == null)
            {
                return false; // Người dùng không tồn tại
            }
            // Mã hóa mật khẩu mới
            //user.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            _userRepository.UpdateUser(user);
            return true;
        }

        //change password
        public async Task<string> ChangePassword(ChangePasswordDto request)
        {
            var user =  _userRepository.GetUserByPhone(request.PhoneNumber);
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
}
