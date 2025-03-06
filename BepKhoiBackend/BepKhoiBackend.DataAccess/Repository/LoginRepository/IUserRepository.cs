using BepKhoiBackend.DataAccess.Models;

namespace BepKhoiBackend.DataAccess.Repository.LoginRepository
{
    public interface IUserRepository
    {
        User? GetUserByPhone(string phone);
    }
}
