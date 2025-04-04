using BepKhoiBackend.BusinessObject.dtos.RoomDto;

namespace BepKhoiBackend.BusinessObject.Services.RoomService
{
    public interface IRoomService
    {
        Task<FileDataDto> DownloadQRCodeAsync(int roomId);

        Task DeleteQRCodeAsync(int roomId);
        Task UpdateQRCodeUrlAsync(int roomId, string qrCodeUrl);
        Task<string> GenerateQRCodeAndSaveAsync(int roomId);
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
