using BepKhoiBackend.BusinessObject.Services.UserService.ManagerService;
using BepKhoiBackend.BusinessObject.dtos.UserDto.ManagerDto;
using Microsoft.AspNetCore.Mvc;

namespace BepKhoiBackend.API.Controllers.UserControllers.ManagerControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManagerController : ControllerBase
    {
        private readonly IManagerService _managerService;

        public ManagerController(IManagerService managerService)
        {
            _managerService = managerService;
        }

        // Lấy thông tin Manager theo ID
        [HttpGet("{id}")]
        public IActionResult GetManagerById(int id)
        {
            var manager = _managerService.GetManagerById(id);
            if (manager == null)
            {
                return NotFound($"Không tìm thấy Manager với ID {id}.");
            }
            return Ok(manager);
        }

        // Cập nhật thông tin Manager
        [HttpPut("{id}")]
        public IActionResult UpdateManager(int id, [FromBody] UpdateManagerDTO updatedManager)
        {
            if (updatedManager == null)
            {
                return BadRequest("Dữ liệu không hợp lệ.");
            }

            var isUpdated = _managerService.UpdateManager(
                id,
                updatedManager.Email,
                updatedManager.Phone,
                updatedManager.UserName,
                updatedManager.Address,
                updatedManager.ProvinceCity,
                updatedManager.District,
                updatedManager.WardCommune,
                updatedManager.DateOfBirth
            );

            if (!isUpdated)
            {
                return BadRequest("Cập nhật Manager thất bại. Kiểm tra lại thông tin.");
            }

            return Ok($"Manager có ID {id} đã được cập nhật thành công.");
        }
    }
}
