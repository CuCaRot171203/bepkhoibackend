using BepKhoiBackend.BusinessObject.dtos.MenuDto;
using BepKhoiBackend.BusinessObject.dtos.MenuPOSDto;
using BepKhoiBackend.BusinessObject.Services.MenuPOSService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BepKhoiBackend.API.Controllers.MenuPOSControllers
{
    [Route("api/menu-pos")]
    [ApiController]
    public class MenuPOSController : ControllerBase
    {
        private readonly IMenuPOSService _menuPOSService;

        public MenuPOSController(IMenuPOSService menuPOSService)
        {
            _menuPOSService = menuPOSService;
        }

        /// <summary>
        /// Lấy danh sách món ăn với phân trang
        /// </summary>
        /// <param name="limit">Số lượng món ăn mỗi trang</param>
        /// <param name="offset">Vị trí bắt đầu lấy dữ liệu</param>
        /// <returns>Danh sách món ăn</returns>
        [HttpGet("get-all")]
        public ActionResult<List<MenuPOSDto>> GetAllMenuPos([FromQuery] int limit = 10, [FromQuery] int offset = 0)
        {
            var result = _menuPOSService.GetAllMenuPos(limit, offset);
            return Ok(result);
        }

        [HttpGet("GetAllProductCategories")]
        public ActionResult<List<ProductCategoryDTO>> GetAllProductCategories()
        {
            var categories = _menuPOSService.GetAllProductCategories();
            return Ok(categories);
        }

        [HttpPost("FilterProductPos")]
        public ActionResult<List<MenuPOSDto>> FilterProductPos([FromBody] FilterProductPosRequest filterRequest)
        {
            var menus = _menuPOSService.FilterProductPos(filterRequest.ProductCategoryId, filterRequest.Limit, filterRequest.Offset);
            return Ok(menus);
        }
    }
}
