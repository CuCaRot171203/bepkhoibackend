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
        private readonly QRCodeService _qrCodeService;
        public RoomController(IRoomService roomService, QRCodeService qrCodeService)
        {
            _roomService = roomService;
            _qrCodeService = qrCodeService;
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

        [HttpPost("generate-qr/{id}")]
        public async Task<IActionResult> GenerateQRCodeForRoom(int id)
        {
            try
            {
                string qrCodeUrl = await _roomService.GenerateQRCodeAndSaveAsync(id);
                return Ok(new { message = "QR Code generated successfully", qrCodeUrl });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Server error", error = ex.Message });
            }
        }

        [HttpDelete("delete-qr/{id}")]
        public async Task<IActionResult> DeleteQRCode(int id)
        {
            try
            {
                await _roomService.DeleteQRCodeAsync(id);
                return Ok(new { message = "QR Code deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("download-qr/{id}")]
        public async Task<IActionResult> DownloadQRCode(int id)
        {
            try
            {
                var fileData = await _roomService.DownloadQRCodeAsync(id);
                return File(fileData.Content, fileData.ContentType, fileData.FileName);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
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


        //controller for get Room for POS site
        [HttpGet("get-all-room-for-pos")]
        [ProducesResponseType(typeof(List<RoomDtoPos>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> TestGetRooms([FromQuery] int limit = 10, [FromQuery] int offset = 0)
        {
            try
            {
                var result = await _roomService.GetRoomAsyncForPos(limit, offset);
                if (result == null || !result.Any())
                {
                    return NotFound(new { message = "Không tìm thấy dữ liệu phòng." });
                }

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "error server.", error = ex.Message });
            }
        }

        // controller for filter by roomAreaId and isUse
        [HttpGet("filter-room-pos")]
        [ProducesResponseType(typeof(List<RoomDtoPos>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> FilterRoomPos([FromQuery] int? roomAreaId, [FromQuery] bool? isUse)
        {
            try
            {
                var result = await _roomService.FilterRoomAsyncPos(roomAreaId, isUse);
                if (result == null || !result.Any())
                {
                    return NotFound(new { message = "Can't find data of room." });
                }

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error server.", error = ex.Message });
            }
        }

        // controller for searching by username and room name
        [HttpGet("search-room-pos")]
        [ProducesResponseType(typeof(List<RoomDtoPos>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> SearchRoomPos([FromQuery] string searchString)
        {
            try
            {
                var result = await _roomService.SearchRoomPosAsync(searchString);
                if (result == null || !result.Any())
                {
                    return NotFound(new { message = "Can't find username or roomname." });
                }

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error server.", error = ex.Message });
            }
        }
    }
}
