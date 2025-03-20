using BepKhoiBackend.DataAccess.Models;
using System;

namespace BepKhoiBackend.DataAccess.Repository.UserRepository.ManagerRepository
{
    public interface IManagerRepository
    {
        User? GetManagerById(int id);
        bool UpdateManager(int userId, string email, string phone, string userName, string address,
                           string provinceCity, string district, string wardCommune, DateTime? dateOfBirth);
    }
}
