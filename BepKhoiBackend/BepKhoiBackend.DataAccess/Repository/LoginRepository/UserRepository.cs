using BepKhoiBackend.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace BepKhoiBackend.DataAccess.Repository.LoginRepository
{
    public class UserRepository : IUserRepository
    {
        private readonly bepkhoiContext _context;

        public UserRepository(bepkhoiContext context)
        {
            _context = context;
        }

        public User? GetUserByPhone(string phone)
        {
            return _context.Users.FirstOrDefault(u => u.Phone == phone);
        }
    }
}
