using BepKhoiBackend.BusinessObject.dtos.UserDto.CashierDto;
using BepKhoiBackend.DataAccess.Models;
using BepKhoiBackend.DataAccess.Repository.UserRepository.CashierRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BepKhoiBackend.BusinessObject.Services.UserService.CashierService
{
    public class CashierService : ICashierService
    {
        private readonly ICashierRepository _cashierRepository;

        public CashierService(ICashierRepository cashierRepository)
        {
            _cashierRepository = cashierRepository;
        }

        public List<CashierDTO> GetAllCashiers()
        {
            var cashiers = _cashierRepository.GetAllCashiers();
            return cashiers
                .Where(c => c.UserInformation != null) // Lọc các Cashier có UserInformation hợp lệ
                .Select(c => new CashierDTO
                {
                    UserId = c.UserId,
                    UserName = c.UserInformation?.UserName ?? "Unknown", // Kiểm tra null
                    Phone = c.Phone,
                    Status = c.Status
                }).ToList();
        }

        public CashierDTO GetCashierById(int id)
        {
            var cashier = _cashierRepository.GetCashierById(id);
            if (cashier == null || cashier.UserInformation == null) return null; // Kiểm tra null

            return new CashierDTO
            {
                UserId = cashier.UserId,
                UserName = cashier.UserInformation?.UserName ?? "Unknown", // Kiểm tra null
                Phone = cashier.Phone,
                Status = cashier.Status
            };
        }
        public void CreateCashier(string email, string password, string phone, string userName)
        {
            _cashierRepository.CreateCashier(email, password, phone, userName);
        }

        public void UpdateCashier(int userId, string email, string phone, string userName)
        {
            _cashierRepository.UpdateCashier(userId, email, phone, userName);
        }

        public void DeleteCashier(int userId)
        {
            _cashierRepository.DeleteCashier(userId);
        }

        public List<CashierInvoiceDTO> GetCashierInvoices(int cashierId)
        {
            var invoices = _cashierRepository.GetCashierInvoices(cashierId);

            return invoices.Select(i => new CashierInvoiceDTO
            {
                InvoiceId = i.InvoiceId,
                CustomerId = i.CustomerId ?? 0,
                CustomerName = i.Customer != null ? i.Customer.CustomerName : "Unknown",
                CheckInTime = i.CheckInTime,
                AmountDue = i.AmountDue,
                Status = i.Status,
                PaymentMethodId = i.PaymentMethodId,
                PaymentMethodName = i.PaymentMethod?.PaymentMethodTitle ?? "Unknown",
                InvoiceDetails = i.InvoiceDetails.Select(d => new CashierInvoiceDetailDTO
                {
                    InvoiceDetailId = d.InvoiceDetailId,
                    ProductId = d.ProductId,
                    ProductName = d.Product?.ProductName ?? "Unknown",
                    Quantity = d.Quantity,
                    ProductPrice = d.Price,
                    ProductVAT = d.ProductVat ?? 0m,
                    ProductNote = d.ProductNote
                }).ToList()
            }).ToList();
        }

        public List<CashierDTO> GetCashiersByNameOrPhone(string searchTerm)
        {
            var cashiers = _cashierRepository.GetCashiersByNameOrPhone(searchTerm);
            return cashiers
                .Where(c => c.UserInformation != null)
                .Select(c => new CashierDTO
                {
                    UserId = c.UserId,
                    UserName = c.UserInformation?.UserName ?? "Unknown",
                    Phone = c.Phone,
                    Status = c.Status
                }).ToList();
        }

        public List<CashierDTO> GetCashierByStatus(bool status)
        {
            return _cashierRepository.GetCashiersSortedByStatus(status)
                .Where(c => c.UserInformation != null)
                .Select(c => new CashierDTO
                {
                    UserId = c.UserId,
                    UserName = c.UserInformation?.UserName ?? "Unknown",
                    Status = c.Status,
                    Phone = c.Phone
                }).ToList();
        }
    }
}
