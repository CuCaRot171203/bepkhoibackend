using BepKhoiBackend.BusinessObject.Services.LoginService.Interface;
using BepKhoiBackend.BusinessObject.Services.UserService.ShipperService;
using Microsoft.AspNetCore.Mvc;

namespace BepKhoiBackend.API.Controllers.UserControllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }


        [HttpGet("get-user-by-id/{userId}")]
        public async Task<IActionResult> GetUserById(int userId)
        {
            var userDto = await _userService.GetUserByIdAsync(userId);

            if (userDto == null)
                return NotFound(new { message = "User not found" });

            return Ok(new
            {
                message = "User fetched successfully",
                data = userDto
            });
        }

    }
}
