using BepKhoiBackend.BusinessObject.dtos.UserDto.ManagerDto;
using BepKhoiBackend.DataAccess.Models;
using BepKhoiBackend.DataAccess.Repository.UserRepository.ManagerRepository;
using System;

namespace BepKhoiBackend.BusinessObject.Services.UserService.ManagerService
{
    public class ManagerService : IManagerService
    {
        private readonly IManagerRepository _managerRepository;

        public ManagerService(IManagerRepository managerRepository)
        {
            _managerRepository = managerRepository;
        }

        public GetManagerDTO GetManagerById(int id)
        {
            var manager = _managerRepository.GetManagerById(id);
            if (manager == null || manager.UserInformation == null) return null; // Kiểm tra null

            return new GetManagerDTO
            {
                UserId = manager.UserId,
                UserName = manager.UserInformation?.UserName ?? "Unknown", // Kiểm tra null
                RoleName = manager.Role?.RoleName ?? "Unknown",
                Phone = manager.Phone ?? "N/A",
                Email = manager.Email ?? "N/A",
                Address = manager.UserInformation?.Address ?? "N/A",
                Province_City = manager.UserInformation?.ProvinceCity ?? "N/A",
                District = manager.UserInformation?.District ?? "N/A",
                Ward_Commune = manager.UserInformation?.WardCommune ?? "N/A",
                Date_of_Birth = manager.UserInformation?.DateOfBirth ?? null,
            };
        }


        public bool UpdateManager(int userId, string email, string phone, string userName, string address,
                                  string provinceCity, string district, string wardCommune, DateTime? dateOfBirth)
        {
            return _managerRepository.UpdateManager(userId, email, phone, userName, address, provinceCity, district, wardCommune, dateOfBirth);
        }
    }
}
