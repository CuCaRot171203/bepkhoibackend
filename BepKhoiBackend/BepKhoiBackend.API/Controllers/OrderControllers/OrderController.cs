﻿using BepKhoiBackend.BusinessObject.Abstract.OrderAbstract;
using BepKhoiBackend.BusinessObject.dtos.CustomerDto;
using BepKhoiBackend.BusinessObject.dtos.MenuDto;
using BepKhoiBackend.BusinessObject.dtos.OrderDetailDto;
using BepKhoiBackend.BusinessObject.dtos.OrderDto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace BepKhoiBackend.API.Controllers.OrderControllers
{

    [ApiController]
    [Route("api/orders")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly PrintOrderPdfService _printOrderPdfService;
        public OrderController(IOrderService orderService, PrintOrderPdfService printOrderPdfService)
        {
            _orderService = orderService;
            _printOrderPdfService = printOrderPdfService;
        }
        //get all
        [HttpGet("get-all-orders")]
        public async Task<IActionResult> GetAllOrdersAsync()
        {
            var result = await _orderService.GetAllOrdersAsync();

            if (!result.IsSuccess)
                return NotFound(new { message = result.Message });

            return Ok(new
            {
                message = result.Message,
                data = result.Data
            });
        }
        // Create order
        [HttpPost("create-order")]
        public async Task<IActionResult> CreateNewOrder([FromBody] CreateOrderRequestDto request)
        {
            try
            {
                var result = await _orderService.CreateNewOrderAsync(request);
                return Ok(new { message = "Order created successfully", data = result });
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

        // Method to add note to OrderId
        [HttpPut("add-note")]
        [ProducesResponseType(200)] // OK
        [ProducesResponseType(400)] // BadRequest
        [ProducesResponseType(404)] // NotFound
        [ProducesResponseType(500)] // Server Error
        public async Task<IActionResult> AddNoteToOrder([FromBody] AddNoteRequest request)
        {
            try
            {
                // check order Id valid
                if (request.OrderId <= 0)
                {
                    return BadRequest(new { message = "Invalid Order ID." });
                }

                var result = await _orderService.AddOrderNoteToOrderPosAsync(request);
                return Ok(new { message = "Note added successfully", data = result });
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

        [HttpPut("update-order-detail-quantity")]
        [ProducesResponseType(200)] // OK
        [ProducesResponseType(400)] // BadRequest
        [ProducesResponseType(404)] // NotFound
        [ProducesResponseType(500)] // Internal Server Error
        public async Task<IActionResult> UpdateOrderDetailQuantity([FromBody] UpdateOrderDetailQuantityRequest request)
        {
            try
            {
                if (request.OrderId <= 0 || request.OrderDetailId <= 0)
                {
                    return BadRequest(new { message = "Invalid Order ID or Order Detail ID." });
                }

                var result = await _orderService.UpdateOrderDetailQuantiyPosAsync(request);
                return Ok(new { message = "Order detail updated successfully", data = result });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
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

        [HttpPost("add-customer-to-order")]
        [ProducesResponseType(200)] // OK
        [ProducesResponseType(400)] // Bad Request
        [ProducesResponseType(404)] // Not Found
        [ProducesResponseType(500)] // Internal Server Error
        public async Task<IActionResult> AddCustomerToOrderPosSite([FromBody] AddCustomerToOrderRequest request)
        {
            try
            {
                if (request.OrderId <= 0 || request.CustomerId <= 0)
                {
                    return BadRequest(new { message = "Invalid input parameters." });
                }

                var result = await _orderService.AddCustomerToOrderAsync(request);

                return Ok(new { message = "Customer added to order successfully", data = result });
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

        [HttpPost("add-product-to-order")]
        [ProducesResponseType(200)] // OK
        [ProducesResponseType(400)] // Bad Request
        [ProducesResponseType(404)] // Not Found
        [ProducesResponseType(500)] // Internal Server Error
        public async Task<IActionResult> AddProductToOrderPosSite([FromBody] AddProductToOrderRequest request)
        {
            try
            {
                if (request.OrderId <= 0 || request.ProductId <= 0)
                {
                    return BadRequest(new { message = "Invalid input parameters." });
                }

                var result = await _orderService.AddProductToOrderAsync(request);

                return Ok(new { message = "Product added to order successfully", data = result });
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


        //Pham Son Tung
        [HttpPut("MoveOrderPos")]
        public async Task<IActionResult> UpdateOrderType([FromBody] MoveOrderPosRequestDto request)
        {
            try
            {
                bool result = await _orderService.ChangeOrderTypeServiceAsync(request);
                return result
                    ? Ok(new { message = "Order type updated successfully." })
                    : BadRequest(new { message = "Failed to update order type." });
            }
            catch (ArgumentException ex) // Lỗi do tham số đầu vào không hợp lệ (service)
            {
                return BadRequest(new { message = "Invalid input parameters.", error = ex.Message });
            }
            catch (KeyNotFoundException ex) // Lỗi do không tìm thấy Order, Room hoặc User (repo)
            {
                return NotFound(new { message = "Resource not found.", error = ex.Message });
            }
            catch (Exception ex) // Các lỗi khác (bao gồm lỗi ở repository)
            {
                return StatusCode(500, new { message = "An internal server error occurred.", error = ex.Message });
            }
        }

        //Pham Son Tung
        [HttpPut("combine-orders")]
        public async Task<IActionResult> CombineOrderPosAsync([FromBody] CombineOrderPosRequestDto request)
        {
            try
            {
                bool result = await _orderService.CombineOrderPosServiceAsync(request);

                return result
                    ? Ok(new { message = "Orders combined successfully." })
                    : BadRequest(new { message = "Failed to combine orders." });
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An internal server error occurred.", error = ex.Message });
            }
        }

        //Pham Son Tung
        [HttpGet("get-order-by-type-pos")]
        public async Task<IActionResult> GetOrdersByTypePosAsync(int? roomId, int? shipperId, int? orderTypeId)
        {
            try
            {
                // Gọi service để lấy danh sách đơn hàng
                var orders = await _orderService.GetOrdersByTypePosAsync(roomId, shipperId, orderTypeId);

                // Trả về kết quả thành công với danh sách đơn hàng
                return Ok(orders);
            }
            catch (ArgumentException argEx)
            {
                // Nếu có lỗi về tham số, trả về lỗi với thông điệp chi tiết
                return BadRequest(new { message = $"Invalid parameter: {argEx.Message}" });
            }
            catch (Exception ex)
            {
                // Xử lý tất cả các lỗi khác, trả về lỗi server
                return StatusCode(500, new { message = "An error occurred while processing your request.", details = ex.Message });
            }
        }

        //-------------NgocQuan----------------//
        [HttpGet("{orderId}/print-pdf-temp-Invoice")]
        public async Task<IActionResult> GetTempInvoicePdf(int orderId)
        {
            try
            {
                var pdfBytes = await _printOrderPdfService.GenerateTempInvoicePdfAsync(orderId);
                return File(pdfBytes, "application/pdf", $"Invoice_{orderId}.pdf");
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
        //Pham Son Tung
        [HttpGet("get-customer-of-order/{orderId}")]
        public async Task<IActionResult> GetCustomerOfOrder([FromRoute] int orderId)
        {
            try
            {
                var customer = await _orderService.GetCustomerIdByOrderIdAsync(orderId); // Trả về CustomerPosDto

                return Ok(new
                {
                    success = true,
                    data = customer
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    success = false,
                    message = ex.Message
                });
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "A database update error occurred.",
                    details = ex.Message
                });
            }
            catch (DbException ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "A general database error occurred.",
                    details = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An unexpected error occurred.",
                    details = ex.Message
                });
            }
        }

        //Pham Son Tung
        [HttpPost("assign-customer-to-order")]
        public async Task<IActionResult> AssignCustomerToOrder(
        [FromQuery] int orderId,
        [FromQuery] int customerId)
        {
            try
            {
                await _orderService.AssignCustomerToOrderAsync(orderId, customerId);

                return Ok(new
                {
                    success = true,
                    message = $"Customer {customerId} has been assigned to order {orderId}."
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    success = false,
                    message = ex.Message
                });
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "A database update error occurred.",
                    details = ex.Message
                });
            }
            catch (DbException ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "A general database error occurred.",
                    details = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An unexpected error occurred.",
                    details = ex.Message
                });
            }
        }
    }
}