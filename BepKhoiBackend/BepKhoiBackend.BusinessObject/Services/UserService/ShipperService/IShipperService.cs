using BepKhoiBackend.BusinessObject.dtos.UserDto.ShipperDto;
using System.Collections.Generic;

namespace BepKhoiBackend.BusinessObject.Services.UserService.ShipperService
{
    public interface IShipperService
    {
        List<ShipperDTO> GetAllShippers();
        ShipperDTO? GetShipperById(int id);
        void CreateShipper(string email, string password, string phone, string userName);
        void UpdateShipper(int userId, string email, string phone, string userName);
        void DeleteShipper(int userId);
        List<ShipperInvoiceDTO> GetShipperInvoices(int shipperId);
        List<ShipperDTO> GetShippersByNameOrPhone(string searchTerm);
        List<ShipperDTO> GetShipperByStatus(bool status);
    }
}
