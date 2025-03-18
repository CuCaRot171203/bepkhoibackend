using BepKhoiBackend.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BepKhoiBackend.DataAccess.Repository.UserRepository.CashierRepository
{
    public interface ICashierRepository
    {
        List<User> GetAllCashiers();
        User? GetCashierById(int id);
        void CreateCashier(string email, string password, string phone, string userName);
        void UpdateCashier(int userId, string email, string phone, string userName);
        void DeleteCashier(int userId);
        List<Invoice> GetCashierInvoices(int cashierId);
        List<User> GetCashiersByNameOrPhone(string searchTerm);
        List<User> GetCashiersSortedByStatus(bool status);

    }
}
