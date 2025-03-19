using AutoMapper;
using BepKhoiBackend.DataAccess.Abstract.MenuAbstract;
using BepKhoiBackend.DataAccess.Models;
using BepKhoiBackend.BusinessObject.dtos.MenuDto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

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

        // API to get all menus with optional filters, pagination, and sorting
        [HttpGet("menus")]
        [ProducesResponseType(200)] // OK
        [ProducesResponseType(400)] // BadRequest
        [ProducesResponseType(500)] // InternalServerError
        public async Task<IActionResult> GetAllMenus(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string sortBy = "ProductId",
            [FromQuery] string sortDirection = "asc",
            [FromQuery] int? categoryId = null,
            [FromQuery] bool? isActive = null,
            [FromQuery] string? productNameOrId = null)
        {
            try
            {
                if (page <= 0 || pageSize <= 0)
                {
                    return BadRequest(new { message = "Page and PageSize must be greater than 0." });
                }

                var result = await _menuRepository.GetAllMenus(
                    page,
                    pageSize,
                    sortBy,
                    sortDirection,
                    categoryId,
                    isActive,
                    productNameOrId);

                if (!result.IsSuccess)
                {
                    return NotFound(new { message = result.Message });
                }

                // Map data to DTO
                var mappedData = _mapper.Map<IEnumerable<MenuDto>>(result.Data);

                // Return success response with pagination info
                return Ok(new
                {
                    message = result.Message,
                    data = mappedData,
                    page = result.Page,
                    pageSize = result.PageSize,
                    totalRecords = result.TotalRecords
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the menu list.");
                return StatusCode(500, new { message = "An unexpected error occurred while retrieving the menu list." });
            }
        }

        // API Get menu by ID
        [HttpGet("{id}")]
        [ProducesResponseType(200)] // OK
        [ProducesResponseType(400)] // BadRequest
        [ProducesResponseType(404)] // NotFound
        [ProducesResponseType(500)] // InternalServerError
        public async Task<IActionResult> GetMenuById(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { message = "Product ID must be greater than 0." });
                }
                var result = await _menuRepository.GetMenuById(id);

                // Check if not founded data
                if (!result.IsSuccess || result.Data == null || !result.Data.Any())
                {
                    _logger.LogWarning($"Couldn't find menu with ID: {id}");
                    return NotFound(new { message = result.Message });
                }
                var mappedData = _mapper.Map<MenuDto>(result.Data.First());

                return Ok(new
                {
                    message = result.Message,
                    data = mappedData
                });
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, $"Invalid argument when finding menu with ID: {id}");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving menu with ID: {id}");
                return StatusCode(500, new { message = "An unexpected error occurred while retrieving the menu." });
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
                // Check menu invalid
                if (menuDto == null)
                {
                    _logger.LogWarning("Invalid menu data received.");
                    return BadRequest(new { message = "Menu data is invalid." });
                }
                var menu = _mapper.Map<Menu>(menuDto);

                var result = await _menuRepository.AddMenu(menu);

                // Check if call function false
                if (!result.IsSuccess)
                {
                    _logger.LogWarning($"Failed to add menu: {result.Message}");
                    return BadRequest(new { message = result.Message });
                }

                var addedMenuDto = _mapper.Map<MenuDto>(result.Data.FirstOrDefault());

                return CreatedAtAction(nameof(GetMenuById), new { id = addedMenuDto.ProductId }, new
                {
                    message = result.Message,
                    data = addedMenuDto
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding the menu.");
                return StatusCode(500, new { message = "An unexpected error occurred while adding the menu." });
            }
        }

        [HttpPut("{productId}")]
        [ProducesResponseType(200)] // OK
        [ProducesResponseType(400)] // Bad Request
        [ProducesResponseType(404)] // Not Found
        [ProducesResponseType(500)] // Internal Server Error
        public async Task<IActionResult> UpdateMenuById(int productId, [FromBody] MenuDto menuDto)
        {
            try
            {
                // check exist
                if (menuDto == null || productId != menuDto.ProductId)
                {
                    _logger.LogWarning("Product ID does not match or data is invalid.");
                    return BadRequest(new { message = "Product ID does not match or data is invalid." });
                }

                var existingMenuResult = await _menuRepository.GetMenuById(productId);
                // check null
                if (!existingMenuResult.IsSuccess || existingMenuResult.Data.IsNullOrEmpty() || existingMenuResult.Data.Count() == 0)
                {
                    _logger.LogWarning($"Menu with ID {productId} not found or has been deleted.");
                    return NotFound(new { message = $"Menu with ID {productId} not found or has been deleted." });
                }

                var updatedMenu = _mapper.Map<Menu>(menuDto);

                var updateResult = await _menuRepository.UpdateMenu(updatedMenu);

                // Check if update was successful
                if (!updateResult.IsSuccess)
                {
                    _logger.LogError($"Failed to update menu with ID {productId}: {updateResult.Message}");
                    return StatusCode(500, new { message = $"Failed to update menu: {updateResult.Message}" });
                }

                return Ok(new
                {
                    message = $"Menu with ID {productId} updated successfully.",
                    data = _mapper.Map<MenuDto>(updateResult.Data.FirstOrDefault())
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while updating menu with ID {productId}.");
                return StatusCode(500, new { message = "An error occurred while updating menu." });
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

        // API Search menu by ProductId or ProductName
        [HttpGet("search")]
        [ProducesResponseType(200)] // OK
        [ProducesResponseType(400)] // Bad Request
        [ProducesResponseType(404)] // Not Found
        [ProducesResponseType(500)] // Internal Server Error
        public async Task<IActionResult> SearchByNameOrProductId(
            [FromQuery] string stringInput,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string sortBy = "ProductId", 
            [FromQuery] string sortDirection = "asc",
            [FromQuery] int? categoryId = null
        )
        {
            try
            {
                // Check input is null or whitespace
                if (string.IsNullOrWhiteSpace(stringInput))
                {
                    _logger.LogWarning("Search input is null or empty.");
                    return BadRequest(new { message = "Search input cannot be empty." });
                }

                // Check pagination parameters are valid
                if (page <= 0 || pageSize <= 0)
                {
                    _logger.LogWarning("Invalid pagination parameters.");
                    return BadRequest(new { message = "Page and PageSize must be greater than 0." });
                }

                // Call repository to search product
                var result = await _menuRepository.SearchProductByNameOrId(stringInput, page, pageSize, sortBy, sortDirection, categoryId);

                // Check result success or fail
                if (!result.IsSuccess || result.Data == null || !result.Data.Any())
                {
                    _logger.LogWarning($"No products found for input: {stringInput}");
                    return NotFound(new { message = result.Message });
                }

                // Map to DTO
                var mappedData = _mapper.Map<IEnumerable<MenuDto>>(result.Data);

                // Return with pagination info
                return Ok(new
                {
                    message = result.Message,
                    data = mappedData,
                    page = result.Page,
                    pageSize = result.PageSize,
                    totalRecords = result.TotalRecords
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while searching for product with input: {stringInput}");
                return StatusCode(500, new { message = "An unexpected error occurred while searching for products." });
            }
        }

        // API to get list products are active in database
        [HttpGet("active-products")]
        [ProducesResponseType(200)] // OK
        [ProducesResponseType(400)] // BadRequest
        [ProducesResponseType(500)] // InternalServerError
        public async Task<IActionResult> GetActiveProducts(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string sortBy = "ProductId",
            [FromQuery] string sortDirection = "asc",
            [FromQuery] int? categoryId = null)
        {
            try
            {
                // Validate pagination input
                if (page <= 0 || pageSize <= 0)
                {
                    return BadRequest(new { message = "Page and PageSize must be greater than 0." });
                }

                // Call service to get data
                var result = await _menuRepository.GetActiveProductsList(page, pageSize, sortBy, sortDirection, categoryId);

                // Handle result
                if (!result.IsSuccess)
                {
                    return NotFound(new { message = result.Message });
                }

                // Map to DTO
                var mappedData = _mapper.Map<IEnumerable<MenuDto>>(result.Data);

                // Return response
                return Ok(new
                {
                    message = result.Message,
                    data = mappedData,
                    page = result.Page,
                    pageSize = result.PageSize,
                    totalRecords = result.TotalRecords
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving active products.");
                return StatusCode(500, new { message = "An unexpected error occurred while retrieving products." });
            }
        }

        // API to get excel file
        [HttpGet("export-products-excel")]
        public async Task<IActionResult> ExportProductsToExcel(
            [FromQuery] string sortBy = "ProductId",
            [FromQuery] string sortDirection = "asc",
            [FromQuery] int? categoryId = null,
            [FromQuery] bool? isActive = null
        )
        {
            var (fileContent, fileName, hasData, errorMessage) = await _menuRepository.ExportActiveProductsToExcelAsync(sortBy, sortDirection, categoryId, isActive);

            // Check null
            if (!string.IsNullOrEmpty(errorMessage))
            {
                // Check exist
                if (errorMessage.Contains("does not exist"))
                    return BadRequest(new { message = errorMessage });
                // Check data
                if (errorMessage.Contains("No product data found"))
                    return NotFound(new { message = errorMessage });

                return StatusCode(500, new { message = errorMessage });
            }

            return File(
                fileContent,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName
            );
        }


        // API to update price of a product 
        [HttpPut("update-price/{productId}")]
        [ProducesResponseType(200)] // OK
        [ProducesResponseType(400)] // Bad Request
        [ProducesResponseType(404)] // Not Found
        [ProducesResponseType(500)] // Internal Server Error
        public async Task<IActionResult> UpdatePriceOfProduct(int productId, [FromBody] UpdatePriceDto priceDto)
        {
            try
            {
                // check null or not matched
                if (priceDto == null || productId != priceDto.ProductId)
                {
                    _logger.LogWarning("Product ID does not match or data is invalid.");
                    return BadRequest(new { message = "Product ID does not match or data is invalid." });
                }

                // update
                var result = await _menuRepository.UpdatePriceOfProduct(
                    productId,
                    priceDto.CostPrice,
                    priceDto.SellPrice,
                    priceDto.SalePrice,
                    priceDto.ProductVat
                );

                // Check success
                if (!result.IsSuccess)
                {
                    _logger.LogWarning(result.Message);
                    if (result.Message.Contains("does not exist") || result.Message.Contains("deleted"))
                        return NotFound(new { message = result.Message });
                    else
                        return BadRequest(new { message = result.Message });
                }

                return Ok(new { message = "Update product price successfully.", data = result.Data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error when updating product price with ID: {productId}");
                return StatusCode(500, new { message = "Internal server error while updating product price." });
            }
        }

        // API to export product price to Excel file
        [HttpGet("export-product-price-excel")]
        public async Task<IActionResult> ExportProductPriceToExcel(
            [FromQuery] string sortBy = "ProductId",
            [FromQuery] string sortDirection = "asc",
            [FromQuery] int? categoryId = null,
            [FromQuery] bool? isActive = null
        )
        {
            var (fileContent, fileName, hasData, errorMessage) = await _menuRepository.ExportPriceExcelAsync(sortBy, sortDirection, categoryId, isActive);
            // Check null
            if (!string.IsNullOrEmpty(errorMessage))
            {
                // Check exist
                if (errorMessage.Contains("does not exist"))
                    return BadRequest(new { message = errorMessage }); 
                // Check data in
                if (errorMessage.Contains("No product price data found"))
                    return NotFound(new { message = errorMessage });

                // Other server errors
                return StatusCode(500, new { message = errorMessage });
            }

            // Check data in
            if (!hasData || fileContent == null)
            {
                return NotFound(new { message = "No product price data found to export." });
            }

            return File(
                fileContent,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName
            );
        }



    }
}
