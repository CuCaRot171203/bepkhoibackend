using BepKhoiBackend.BusinessObject.dtos.UserDto.CashierDto;
using BepKhoiBackend.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BepKhoiBackend.BusinessObject.Services.UserService.CashierService
{
    public interface ICashierService
    {
        List<CashierDTO> GetAllCashiers();
        CashierDTO? GetCashierById(int id);
        void CreateCashier(string email, string password, string phone, string userName);
        void UpdateCashier(int userId, string email, string phone, string userName);
        void DeleteCashier(int userId);
        List<CashierInvoiceDTO> GetCashierInvoices(int cashierId);
        List<CashierDTO> GetCashiersByNameOrPhone(string searchTerm);
        List<CashierDTO> GetCashierByStatus(bool status);
    }
}
