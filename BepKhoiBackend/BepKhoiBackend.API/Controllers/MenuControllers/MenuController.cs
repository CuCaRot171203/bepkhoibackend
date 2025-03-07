using AutoMapper;
using BepKhoiBackend.DataAccess.Abstract.MenuAbstract;
using BepKhoiBackend.DataAccess.Models;
using BepKhoiBackend.BusinessObject.dtos.MenuDto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BepKhoiBackend.API.Controllers.MenuControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenuController : ControllerBase
    {
        private readonly IMenuRepository _menuRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<MenuController> _logger;

        public MenuController(IMenuRepository menuRepository, IMapper mapper, ILogger<MenuController> logger)
        {
            _menuRepository = menuRepository;
            _mapper = mapper;
            _logger = logger;
        }

        // API Get all menus
        [HttpGet]
        [ProducesResponseType(200)] // OK
        [ProducesResponseType(500)] // Internal Server Error
        public async Task<ActionResult<IEnumerable<MenuDto>>> GetAllMenus()
        {
            try
            {
                var menus = await _menuRepository.GetAllMenus();
                return Ok(_mapper.Map<IEnumerable<MenuDto>>(menus));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when track list products");
                return StatusCode(500, new { message = "Have found error when track list product" });
            }
        }

        // API Get menu by ID
        [HttpGet("{id}")]
        [ProducesResponseType(200)] // OK
        [ProducesResponseType(404)] // Not Found
        [ProducesResponseType(500)] // Internal Server Error
        public async Task<ActionResult<MenuDto>> GetMenuById(int id)
        {
            try
            {
                var menu = await _menuRepository.GetMenuById(id);

                // check menu invalid?
                if (menu == null)
                {
                    _logger.LogWarning($"Couldn't find menu with ID: {id}");
                    return NotFound(new { message = "Couldn't find menu!" });
                }
                return Ok(_mapper.Map<MenuDto>(menu));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $" Error when find menu have ID: {id}");
                return StatusCode(500, new { message = "Have occur error when find menu." });
            }
        }

        // API Add menu
        [HttpPost]
        [ProducesResponseType(201)] // Created
        [ProducesResponseType(400)] // Bad Request
        [ProducesResponseType(500)] // Internal Server Error
        public async Task<IActionResult> AddMenu([FromBody] MenuDto menuDto)
        {
            try
            {
                // check menu invalid?
                if (menuDto == null)
                {
                    _logger.LogWarning("Data of menus are invalid.");
                    return BadRequest(new { message = "Data are invalid." });
                }

                var menu = _mapper.Map<Menu>(menuDto);
                await _menuRepository.AddMenu(menu);
                return CreatedAtAction(nameof(GetMenuById), new { id = menu.ProductId }, new
                {
                    message = "Product added successfully!",
                    data = _mapper.Map<MenuDto>(menu)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when add new product.");
                return StatusCode(500, new { message = "Error when add menu." });
            }
        }

        // API Update menu
        [HttpPut("{productId}")]
        [ProducesResponseType(204)] // No Content
        [ProducesResponseType(400)] // Bad Request
        [ProducesResponseType(404)] // Not Found
        [ProducesResponseType(500)] // Internal Server Error
        public async Task<IActionResult> UpdateMenu(int productId, [FromBody] MenuDto menuDto)
        {
            try
            {
                // check menu invalid? or productId invalid?
                if (menuDto == null || productId != menuDto.ProductId)
                {
                    _logger.LogWarning("ID are not match or data are invalid.");
                    return BadRequest(new { message = "ID are not match or data are invalid." });
                }

                var existingMenu = await _menuRepository.GetMenuById(productId);
                if (existingMenu == null)
                {
                    _logger.LogWarning($"Could't find product with ID: {productId}");
                    return NotFound(new { message = "couldn't find product." });
                }

                // Map DTO to menu object
                var updatedMenu = _mapper.Map<Menu>(menuDto);

                // update data
                await _menuRepository.UpdateMenu(updatedMenu);

                return Ok(new { message = "Update product successfully." }); ;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error when update menu have ID: {productId}");
                return StatusCode(500, new { message = "Error when update menu." });
            }
        }

        // API Delete menu
        [HttpDelete("{productId}")]
        [ProducesResponseType(204)] // No Content
        [ProducesResponseType(404)] // Not Found
        [ProducesResponseType(500)] // Internal Server Error
        public async Task<IActionResult> DeleteMenu(int productId)
        {
            try
            {
                var existingMenu = await _menuRepository.GetMenuById(productId);

                // check menu invalid?
                if (existingMenu == null)
                {
                    _logger.LogWarning($"Couldn't find menu with ID: {productId}");
                    return NotFound(new { message = "Couldn't find menu." });
                }

                await _menuRepository.DeleteMenu(productId);
                return Ok(new { message = "Delete product successfully." }); ;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error when delete product have ID: {productId}");
                return StatusCode(500, new { message = "Error when delete product" });
            }
        }
    }
}
