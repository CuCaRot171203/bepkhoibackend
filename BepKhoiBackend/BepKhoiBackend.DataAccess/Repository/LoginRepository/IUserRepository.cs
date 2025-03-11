using BepKhoiBackend.DataAccess.Models;

public interface IUserRepository
{
    User GetUserByEmail(string email);
    void UpdateUser(User user);
}
