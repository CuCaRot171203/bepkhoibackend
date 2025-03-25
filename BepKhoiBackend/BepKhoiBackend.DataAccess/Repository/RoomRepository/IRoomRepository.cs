using BepKhoiBackend.DataAccess.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BepKhoiBackend.DataAccess.Repository.RoomRepository.Interface
{
    public interface IRoomRepository
    {
        Task<IEnumerable<Room>> GetAllAsync(int limit, int offset);
        Task<Room> GetByIdAsync(int id);
        Task<IEnumerable<Room>> SearchByIdOrNameAsync(string keyword);
        Task AddAsync(Room room);
        Task UpdateAsync(Room room);
        Task<bool> SoftDeleteAsync(int id);

        Task<IEnumerable<Room>> SearchByNameAsync(string roomName, int limit, int offset);        // ✅ Thêm hàm Search
        Task<List<Room>> GetRoomsAsyncPOS(int limit, int offset);
        Task<List<Room>> FilterRoomPosAsync(int? roolAreaId, bool? isUse);
        Task<List<Room>> SearchRoomPosAsync(string searchString);
    }
}
