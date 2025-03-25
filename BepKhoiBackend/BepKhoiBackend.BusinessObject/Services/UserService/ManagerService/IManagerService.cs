using BepKhoiBackend.BusinessObject.dtos.UserDto.ManagerDto;
using System;

namespace BepKhoiBackend.BusinessObject.Services.UserService.ManagerService
{
    public interface IManagerService
    {
        GetManagerDTO? GetManagerById(int id);
        bool UpdateManager(int userId, string email, string phone, string userName,
                         string address, string provinceCity, string district,
                         string wardCommune, DateTime? dateOfBirth);
    }
}
