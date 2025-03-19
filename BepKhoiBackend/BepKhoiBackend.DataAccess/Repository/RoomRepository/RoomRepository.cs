using BepKhoiBackend.DataAccess.Models;
using BepKhoiBackend.DataAccess.Repository.RoomRepository.Interface;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BepKhoiBackend.DataAccess.Repository.RoomRepository
{
    public class RoomRepository : IRoomRepository
    {
        private readonly bepkhoiContext _context;

        public RoomRepository(bepkhoiContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Room>> GetAllAsync(int limit, int offset)
        {
            return await _context.Rooms
                .Where(r => r.IsDelete == false)
                .OrderBy(r => r.RoomId)
                .Skip(offset)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<Room> GetByIdAsync(int id)
        {
            return await _context.Rooms
                .FirstOrDefaultAsync(r => r.RoomId == id && r.IsDelete == false);
        }

        public async Task<IEnumerable<Room>> SearchByIdOrNameAsync(string keyword)
        {
            return await _context.Rooms
                .Where(r => r.IsDelete == false &&
                            (r.RoomId.ToString() == keyword || r.RoomName.Contains(keyword)))
                .ToListAsync();
        }

        public async Task AddAsync(Room room)
        {
            await _context.Rooms.AddAsync(room);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Room room)
        {
            _context.Rooms.Update(room);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            bool hasOrders = await _context.Orders.AnyAsync(o => o.RoomId == id);
            if (hasOrders)
            {
                return false;
            }

            var room = await _context.Rooms.FirstOrDefaultAsync(r => r.RoomId == id);
            if (room == null) return false;

            room.IsDelete = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Room>> SearchByNameAsync(string name, int limit, int offset)
        {
            var query = _context.Rooms
                .Where(r => r.IsDelete == false);

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(r => r.RoomName.Contains(name));
            }

            return await query
                .OrderBy(r => r.RoomId)
                .Skip(offset)
                .Take(limit)
                .ToListAsync();
        }



    }
}
