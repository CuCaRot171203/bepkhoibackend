using BepKhoiBackend.BusinessObject.dtos.RoomDto;
using BepKhoiBackend.BusinessObject.Services.RoomService;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BepKhoiBackend.API.Controllers
{
    [Route("api/rooms")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly IRoomService _roomService;

        public RoomController(IRoomService roomService)
        {
            _roomService = roomService;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll([FromQuery] int limit = 10, [FromQuery] int offset = 0)
        {
            var result = await _roomService.GetAllAsync(limit, offset);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var room = await _roomService.GetByIdAsync(id);
            if (room == null)
                return NotFound(new { message = "Room not found" });

            return Ok(room);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] RoomCreateDto roomCreateDto)
        {
            if (roomCreateDto == null)
                return BadRequest(new { message = "Invalid data" });

            await _roomService.AddAsync(roomCreateDto);
            return Ok(new { message = "Room created successfully" });
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] RoomUpdateDto roomUpdateDto)
        {
            if (roomUpdateDto == null)
                return BadRequest(new { message = "Invalid data" });

            await _roomService.UpdateAsync(id, roomUpdateDto);
            return Ok(new { message = "Room updated successfully" });
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            var success = await _roomService.SoftDeleteAsync(id);
            if (!success)
                return BadRequest(new { message = "Room not found or already deleted" });

            return Ok(new { message = "Room deleted successfully" });
        }

        [HttpGet("search-by-name")]
        public async Task<IActionResult> SearchByName([FromQuery] string? name, [FromQuery] int limit = 10, [FromQuery] int offset = 0)
        {
            var result = await _roomService.SearchByNameAsync(name, limit, offset);
            return Ok(result);
        }





    }
}
