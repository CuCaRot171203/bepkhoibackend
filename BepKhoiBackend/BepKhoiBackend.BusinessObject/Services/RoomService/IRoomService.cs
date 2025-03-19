using BepKhoiBackend.BusinessObject.dtos.RoomDto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BepKhoiBackend.BusinessObject.Services.RoomService
{
    public interface IRoomService
    {
        Task<IEnumerable<RoomDto>> GetAllAsync(int limit, int offset);
        Task<RoomDto> GetByIdAsync(int id);
        Task AddAsync(RoomCreateDto roomCreateDto);
        Task UpdateAsync(int id, RoomUpdateDto roomUpdateDto);
        Task<bool> SoftDeleteAsync(int id);
        Task<IEnumerable<RoomDto>> SearchByNameAsync(string roomName, int limit, int offset);        // ✅ API Search

    }
}
