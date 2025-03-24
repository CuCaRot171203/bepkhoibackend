using BepKhoiBackend.BusinessObject.Abstract.OrderAbstract;
using BepKhoiBackend.BusinessObject.Abstract.OrderDetailAbstract;
using BepKhoiBackend.BusinessObject.dtos.OrderDetailDto;
using BepKhoiBackend.BusinessObject.Services.OrderService;
using Microsoft.AspNetCore.Mvc;

namespace BepKhoiBackend.API.Controllers.OrderDetailControllers
{
    [ApiController]
    [Route("api/order-detail")]
    public class OrderDetailControllers : ControllerBase
    {
        private readonly IOrderDetailService _orderDetailService;

        public OrderDetailControllers(IOrderDetailService orderDetailService)
        {
            _orderDetailService = orderDetailService;
        }

        [HttpDelete("cancel-order-detail")]
        [ProducesResponseType(200)] // OK
        [ProducesResponseType(400)] // BadRequest
        [ProducesResponseType(404)] // NotFound
        [ProducesResponseType(500)] // Internal Server Error
        public async Task<IActionResult> CancelOrderDetail([FromBody] CancelOrderDetailRequest request)
        {
            try
            {
                if (request.OrderId <= 0 || request.OrderDetailId <= 0 || request.CashierId <= 0 || request.Quantity <= 0)
                {
                    return BadRequest(new { message = "Invalid input parameters." });
                }

                var result = await _orderDetailService.CancelOrderDetailAsync(request);
                return Ok(new { message = "Order detail canceled successfully", data = result });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Server error", error = ex.Message });
            }
        }

        // API to remove order detail
        [HttpDelete("remove-order-detail")]
        [ProducesResponseType(200)] // OK
        [ProducesResponseType(400)] // BadRequest
        [ProducesResponseType(404)] // NotFound
        [ProducesResponseType(500)] // Internal Server Error
        public async Task<IActionResult> RemoveOrderDetail([FromBody] RemoveOrderDetailRequest request)
        {
            try
            {
                if (request.OrderId <= 0 || request.OrderDetailId <= 0)
                {
                    return BadRequest(new { message = "Invalid Order ID or Order Detail ID." });
                }

                var result = await _orderDetailService.RemoveOrderDetailAsync(request);
                if (!result)
                {
                    return NotFound(new { message = "Order detail not found or cannot be removed." });
                }

                return Ok(new { message = "Order detail removed successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Server error", error = ex.Message });
            }
        }

        [HttpPut("add-note-to-order-detail")]
        [ProducesResponseType(200)] // OK
        [ProducesResponseType(400)] // Bad Request
        [ProducesResponseType(404)] // Not Found
        [ProducesResponseType(500)] // Internal Server Error
        public async Task<IActionResult> AddNoteToOrderDetail([FromBody] AddNoteToOrderDetailRequest request)
        {
            try
            {
                if (request.OrderId <= 0 || request.OrderDetailId <= 0 || string.IsNullOrWhiteSpace(request.Note))
                {
                    return BadRequest(new { message = "Invalid input parameters." });
                }

                var result = await _orderDetailService.AddNoteToOrderDetailAsync(request);
                if (!result)
                {
                    return NotFound(new { message = "Order detail not found." });
                }

                return Ok(new { message = "Note added successfully." });
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
    }
}
