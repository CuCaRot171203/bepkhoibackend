using BepKhoiBackend.DataAccess.Models;
using System.Collections.Generic;

namespace BepKhoiBackend.DataAccess.Repository.UserRepository.ShipperRepository
{
    public interface IShipperRepository
    {
        List<User> GetAllShippers();
        User? GetShipperById(int id);
        void CreateShipper(string email, string password, string phone, string userName);
        void UpdateShipper(int userId, string email, string phone, string userName);
        void DeleteShipper(int userId);
        List<Invoice> GetShipperInvoices(int shipperId);
        List<User> GetShippersByNameOrPhone(string searchTerm);
        List<User> GetShippersSortedByStatus(bool status);
    }
}
