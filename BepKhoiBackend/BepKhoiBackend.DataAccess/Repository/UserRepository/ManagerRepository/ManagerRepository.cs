using BepKhoiBackend.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BepKhoiBackend.DataAccess.Repository.UserRepository.ManagerRepository
{
    public class ManagerRepository : IManagerRepository
    {
        private readonly bepkhoiContext _context;

        public ManagerRepository(bepkhoiContext context)
        {
            _context = context;
        }

        // Lấy thông tin Manager theo ID
        public User GetManagerById(int id)
        {
            return _context.Users
                .Include(u => u.UserInformation)
                .Include(u => u.Role) // Load thông tin vai trò
                .FirstOrDefault(u => u.UserId == id && u.RoleId == 1 && (u.IsDelete == false || u.IsDelete == null));
        }
      
        // Cập nhật thông tin Manager
        public bool UpdateManager(int userId, string email, string phone, string userName, string address,
                                  string provinceCity, string district, string wardCommune, DateTime? dateOfBirth)
        {
            var manager = _context.Users
                .Include(u => u.UserInformation)
                .FirstOrDefault(u => u.UserId == userId && u.RoleId == 1);

            if (manager == null)
            {
                return false; // Không tìm thấy manager
            }

            // Kiểm tra nếu có thay đổi email thì đặt is_verify = false
            if (!string.IsNullOrEmpty(email) && manager.Email != email)
            {
                manager.Email = email;
                manager.IsVerify = false; // Đánh dấu email chưa xác thực
            }

            // Cập nhật các thông tin cơ bản
            manager.Phone = phone;

            if (manager.UserInformation != null)
            {
                manager.UserInformation.UserName = userName;
                manager.UserInformation.Address = address;
                manager.UserInformation.ProvinceCity = provinceCity;
                manager.UserInformation.District = district;
                manager.UserInformation.WardCommune = wardCommune;
                manager.UserInformation.DateOfBirth = dateOfBirth;
            }

            _context.SaveChanges();
            return true;
        }
    }
}
