using BepKhoiBackend.BusinessObject.dtos.RoomDto;
using BepKhoiBackend.DataAccess.Models;
using BepKhoiBackend.DataAccess.Repository.RoomRepository;
using BepKhoiBackend.DataAccess.Repository.RoomRepository.Interface;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BepKhoiBackend.BusinessObject.Services.RoomService
{
    public class RoomService : IRoomService
    {
        private readonly IRoomRepository _roomRepository;

        public RoomService(IRoomRepository roomRepository)
        {
            _roomRepository = roomRepository;
        }

        public async Task<IEnumerable<RoomDto>> GetAllAsync(int limit, int offset)
        {
            var rooms = await _roomRepository.GetAllAsync(limit, offset);
            return rooms.Select(r => new RoomDto
            {
                RoomId = r.RoomId,
                RoomName = r.RoomName,
                RoomAreaId = r.RoomAreaId,
                OrdinalNumber = r.OrdinalNumber,
                SeatNumber = r.SeatNumber,
                RoomNote = r.RoomNote,
                QrCodeUrl = r.QrCodeUrl,
                Status = r.Status,
                IsUse = r.IsUse,
                IsDelete = r.IsDelete ?? false
            }).ToList();
        }

        public async Task<RoomDto> GetByIdAsync(int id)
        {
            var room = await _roomRepository.GetByIdAsync(id);
            if (room == null) return null;

            return new RoomDto
            {
                RoomId = room.RoomId,
                RoomName = room.RoomName,
                RoomAreaId = room.RoomAreaId,
                OrdinalNumber = room.OrdinalNumber,
                SeatNumber = room.SeatNumber,
                RoomNote = room.RoomNote,
                QrCodeUrl = room.QrCodeUrl,
                Status = room.Status,
                IsUse = room.IsUse,
                IsDelete = room.IsDelete ?? false
            };
        }

        public async Task<IEnumerable<RoomDto>> SearchByIdOrNameAsync(string keyword)
        {
            var rooms = await _roomRepository.SearchByIdOrNameAsync(keyword);
            return rooms.Select(r => new RoomDto
            {
                RoomId = r.RoomId,
                RoomName = r.RoomName
            }).ToList();
        }

        public async Task AddAsync(RoomCreateDto roomCreateDto)
        {
            var room = new Room
            {
                RoomName = roomCreateDto.RoomName,
                RoomAreaId = roomCreateDto.RoomAreaId,
                OrdinalNumber = roomCreateDto.OrdinalNumber,
                SeatNumber = roomCreateDto.SeatNumber,
                RoomNote = roomCreateDto.RoomNote,
                QrCodeUrl = roomCreateDto.QrCodeUrl,
                Status = roomCreateDto.Status,
                IsUse = roomCreateDto.IsUse,
                IsDelete = false
            };

            await _roomRepository.AddAsync(room);
        }

        public async Task UpdateAsync(int id, RoomUpdateDto roomUpdateDto)
        {
            var room = await _roomRepository.GetByIdAsync(id);
            if (room == null) return;

            room.RoomName = roomUpdateDto.RoomName;
            room.RoomAreaId = roomUpdateDto.RoomAreaId;
            room.OrdinalNumber = roomUpdateDto.OrdinalNumber;
            room.SeatNumber = roomUpdateDto.SeatNumber;
            room.RoomNote = roomUpdateDto.RoomNote;
            room.QrCodeUrl = roomUpdateDto.QrCodeUrl;
            room.Status = roomUpdateDto.Status;
            room.IsUse = roomUpdateDto.IsUse;
            room.IsDelete = roomUpdateDto.IsDelete ?? false;
            await _roomRepository.UpdateAsync(room);
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            return await _roomRepository.SoftDeleteAsync(id);
        }

        public async Task<IEnumerable<RoomDto>> SearchByNameAsync(string name, int limit, int offset)
        {
            var rooms = await _roomRepository.SearchByNameAsync(name, limit, offset);

            return rooms.Select(r => new RoomDto
            {
                RoomId = r.RoomId,
                RoomName = r.RoomName,
                RoomAreaId = r.RoomAreaId,
                OrdinalNumber = r.OrdinalNumber,
                SeatNumber = r.SeatNumber,
                RoomNote = r.RoomNote,
                QrCodeUrl = r.QrCodeUrl,
                Status = r.Status,
                IsUse = r.IsUse,
                IsDelete = r.IsDelete
            }).ToList();
        }



    }
}
