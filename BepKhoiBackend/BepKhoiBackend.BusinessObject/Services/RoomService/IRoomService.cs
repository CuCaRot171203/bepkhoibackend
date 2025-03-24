using BepKhoiBackend.BusinessObject.dtos.RoomDto;

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
        // Service - Thanh Tung
        Task<List<RoomDtoPos>> GetRoomAsyncForPos(int limit, int offset);
        Task<List<RoomDtoPos>> FilterRoomAsyncPos(int? roomAreaId, bool? isUse);
        Task<List<RoomDtoPos>> SearchRoomPosAsync(string searchString);
    }
}
