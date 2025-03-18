using BepKhoiBackend.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BepKhoiBackend.DataAccess.Repository.UserRepository.ShipperRepository
{
    public class ShipperRepository : IShipperRepository
    {
        private readonly bepkhoiContext _context;

        public ShipperRepository(bepkhoiContext context)
        {
            _context = context;
        }

        // Lấy danh sách tất cả shipper (RoleId = 3)
        public List<User> GetAllShippers()
        {
            return _context.Users
                .Include(u => u.UserInformation)
                .Where(u => u.RoleId == 3 && (u.IsDelete == false || u.IsDelete == null))
                .ToList();
        }

        // Lấy shipper theo ID
        public User GetShipperById(int id)
        {
            return _context.Users
                .Include(u => u.UserInformation)
                .FirstOrDefault(u => u.UserId == id && u.RoleId == 3 && (u.IsDelete == false || u.IsDelete == null));
        }

        // Thêm mới Shipper
        public void CreateShipper(string email, string password, string phone, string userName)
        {
            // Tạo UserInformation trước
            var userInfo = new UserInformation
            {
                UserName = userName
            };

            _context.UserInformations.Add(userInfo);
            _context.SaveChanges(); // Lưu để lấy UserInformationId

            // Tạo User với UserInformationId vừa tạo
            var shipper = new User
            {
                Email = email,
                Password = password,
                Phone = phone,
                RoleId = 3, // Role = Shipper
                UserInformationId = userInfo.UserInformationId,
                Status = true, // Mặc định kích hoạt
                CreateDate = DateTime.UtcNow,
                IsVerify = false,
                IsDelete = false
            };

            _context.Users.Add(shipper);
            _context.SaveChanges();
        }

        // Cập nhật thông tin Shipper
        public void UpdateShipper(int userId, string email, string phone, string userName)
        {
            var shipper = _context.Users
                .Include(u => u.UserInformation)
                .FirstOrDefault(u => u.UserId == userId && u.RoleId == 3);

            if (shipper != null)
            {
                shipper.Email = email;
                shipper.Phone = phone;

                if (shipper.UserInformation != null)
                {
                    shipper.UserInformation.UserName = userName;
                }

                _context.SaveChanges();
            }
        }

        // Xóa Shipper (Đánh dấu IsDelete = true)
        public void DeleteShipper(int userId)
        {
            var shipper = _context.Users
                .FirstOrDefault(u => u.UserId == userId && u.RoleId == 3);

            if (shipper != null)
            {
                shipper.IsDelete = true; // Đánh dấu là đã xóa
                _context.SaveChanges();
            }
        }
        //Lấy hóa đơn của Shipper
        public List<Invoice> GetShipperInvoices(int shipperId)
        {
            return _context.Invoices
                .Include(i => i.Customer)
                .Include(i => i.OrderType)
                .Include(i => i.PaymentMethod) // Lấy thêm PaymentMethod
                .Include(i => i.InvoiceDetails)
                .ThenInclude(d => d.Product)
                .Where(i => i.ShipperId == shipperId && i.OrderTypeId == 2)
                .ToList();
        }
        public List<User> GetShippersByNameOrPhone(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return new List<User>();
            }

            searchTerm = searchTerm.Trim(); // Loại bỏ khoảng trắng không cần thiết
            return _context.Users
                .Include(u => u.UserInformation)
                .Where(u => u.RoleId == 3 &&
                            (EF.Functions.Like(u.UserInformation.UserName, $"%{searchTerm}%") ||
                             EF.Functions.Like(u.Phone, $"%{searchTerm}%")))
                .ToList();
        }
        // Lấy danh sách shipper theo trạng thái (Status)
        public List<User> GetShippersSortedByStatus(bool status)
        {
            return _context.Users
                .Include(u => u.UserInformation)
                .Where(u => u.RoleId == 3 && u.IsDelete == false && u.Status == status)
                .OrderBy(u => u.Status) // Sắp xếp theo Status
                .ToList();
        }

    }
}
