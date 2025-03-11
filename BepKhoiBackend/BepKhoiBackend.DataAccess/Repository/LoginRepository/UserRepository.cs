using BepKhoiBackend.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

public class UserRepository : IUserRepository
{
    private readonly bepkhoiContext _context;

    public UserRepository(bepkhoiContext context)
    {
        _context = context;
    }

    public User GetUserByEmail(string email)
    {
        return _context.Users.FirstOrDefault(u => u.Email == email);
    }

    public void UpdateUser(User user)
    {
        _context.Users.Update(user);
        _context.SaveChanges();
    }
}
