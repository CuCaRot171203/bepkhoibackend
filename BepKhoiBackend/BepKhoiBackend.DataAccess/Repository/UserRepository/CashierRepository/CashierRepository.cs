using BepKhoiBackend.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BepKhoiBackend.DataAccess.Repository.UserRepository.CashierRepository
{
    public class CashierRepository : ICashierRepository
    {
        private readonly bepkhoiContext _context;

        public CashierRepository(bepkhoiContext context)
        {
            _context = context;
        }

        public List<User> GetAllCashiers()
        {
            return _context.Users
                .Include(u => u.UserInformation) // Load thông tin người dùng
                .Where(u => u.RoleId == 2 && (u.IsDelete == false || u.IsDelete == null))
                .ToList();
        }

        public User GetCashierById(int id)
        {
            return _context.Users
                .Include(u => u.UserInformation) // Load thông tin người dùng
                .FirstOrDefault(u => u.UserId == id && u.RoleId == 2 && (u.IsDelete == false || u.IsDelete == null));
        }

        // Thêm mới Cashier
        public void CreateCashier(string email, string password, string phone, string userName)
        {
            var userInfo = new UserInformation
            {
                UserName = userName
            };

            _context.UserInformations.Add(userInfo);
            _context.SaveChanges();

            var cashier = new User
            {
                Email = email,
                Password = password,
                Phone = phone,
                RoleId = 2, // Role = Cashier
                UserInformationId = userInfo.UserInformationId,
                Status = true,
                CreateDate = DateTime.UtcNow,
                IsVerify = false,
                IsDelete = false
            };

            _context.Users.Add(cashier);
            _context.SaveChanges();
        }

        // Cập nhật thông tin Cashier
        public void UpdateCashier(int userId, string email, string phone, string userName)
        {
            var cashier = _context.Users
                .Include(u => u.UserInformation)
                .FirstOrDefault(u => u.UserId == userId && u.RoleId == 2);

            if (cashier != null)
            {
                cashier.Email = email;
                cashier.Phone = phone;

                if (cashier.UserInformation != null)
                {
                    cashier.UserInformation.UserName = userName;
                }

                _context.SaveChanges();
            }
        }

        // Xóa Cashier (Đánh dấu IsDelete = true)
        public void DeleteCashier(int userId)
        {
            var cashier = _context.Users
                .FirstOrDefault(u => u.UserId == userId && u.RoleId == 2);

            if (cashier != null)
            {
                cashier.IsDelete = true;
                _context.SaveChanges();
            }
        }

        // Lấy hóa đơn của Cashier
        public List<Invoice> GetCashierInvoices(int cashierId)
        {
            return _context.Invoices
                .Include(i => i.Customer)
                .Include(i => i.OrderType)
                .Include(i => i.PaymentMethod)
                .Include(i => i.InvoiceDetails)
                .ThenInclude(d => d.Product)
                .Where(i => i.CashierId == cashierId && i.OrderTypeId == 2)
                .ToList();
        }

        // Tìm Cashier theo tên hoặc số điện thoại
        public List<User> GetCashiersByNameOrPhone(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return new List<User>();
            }

            searchTerm = searchTerm.Trim();
            return _context.Users
                .Include(u => u.UserInformation)
                .Where(u => u.RoleId == 2 &&
                            (EF.Functions.Like(u.UserInformation.UserName, $"%{searchTerm}%") ||
                             EF.Functions.Like(u.Phone, $"%{searchTerm}%")))
                .ToList();
        }

        // Lấy danh sách Cashier theo trạng thái (Status)
        public List<User> GetCashiersSortedByStatus(bool status)
        {
            return _context.Users
                .Include(u => u.UserInformation)
                .Where(u => u.RoleId == 2 && u.IsDelete == false && u.Status == status)
                .OrderBy(u => u.Status)
                .ToList();
        }
    }
}
