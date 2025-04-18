using BepKhoiBackend.BusinessObject.Abstract.OrderAbstract;
using BepKhoiBackend.BusinessObject.dtos.CustomerDto;
using BepKhoiBackend.BusinessObject.dtos.MenuDto;
using BepKhoiBackend.BusinessObject.dtos.OrderDetailDto;
using BepKhoiBackend.BusinessObject.dtos.OrderDto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
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

        [HttpGet("filter-by-date")]
        public async Task<IActionResult> FilterOrdersByDateAsync([FromQuery] DateTime fromDate, [FromQuery] DateTime toDate)
        {
            var result = await _orderService.FilterOrdersByDateAsync(fromDate, toDate);

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
                var orders = await _orderService.GetOrdersByTypePosAsync(roomId, shipperId, orderTypeId);
                return Ok(orders);
            }
            catch (ArgumentException argEx)
            {
                return BadRequest(new
                {
                    message = $"Invalid parameter: {argEx.Message}"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An error occurred while processing your request.",
                    details = ex.InnerException?.Message ?? ex.Message
                });
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
                return Ok(new
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

        //pham son tung
        [HttpPost("remove-customer/{orderId}")]
        public async Task<IActionResult> RemoveCustomerFromOrder(int orderId)
        {
            // Gọi service để thực hiện xóa CustomerId khỏi đơn hàng
            var result = await _orderService.RemoveCustomerFromOrderAsync(orderId);

            // Kiểm tra kết quả và trả về phản hồi cho client
            if (result)
            {
                // Nếu thành công
                return Ok(new { Message = "Customer removed successfully from the order." });
            }
            else
            {
                // Nếu thất bại, có thể vì đơn hàng không tồn tại hoặc gặp lỗi
                return BadRequest(new { Message = "Failed to remove customer from the order." });
            }
        }

        //pham son tung
        [HttpPost("remove-order/{orderId}")]
        public async Task<IActionResult> RemoveOrder(int orderId)
        {
            try
            {
                // Gọi hàm service để thực hiện xóa đơn hàng
                bool result = await _orderService.RemoveOrderById(orderId);

                // Kiểm tra kết quả từ service
                if (result)
                {
                    // Trả về phản hồi thành công nếu đơn hàng được xóa thành công
                    return Ok(new { Message = "Order has been successfully removed." });
                }
                else
                {
                    // Nếu không thành công, trả về lỗi BadRequest
                    return BadRequest(new { Message = "Failed to remove order." });
                }
            }
            catch (Exception ex)
            {
                // Nếu có lỗi trong quá trình xử lý, trả về lỗi 500 (Internal Server Error)
                return StatusCode(500, new { Message = $"Internal server error: {ex.Message}" });
            }
        }

        //pham son tung
        [HttpGet("get-order-details-by-order-id")]
        public async Task<IActionResult> GetOrderDetailsByOrderIdAsync(int orderId)
        {
            try
            {
                // Gọi service để lấy danh sách order details theo orderId
                var orderDetails = await _orderService.GetOrderDetailsByOrderIdAsync(orderId);

                // Trả về kết quả thành công với danh sách order details
                return Ok(orderDetails);
            }
            catch (ArgumentException argEx)
            {
                // Nếu có lỗi về tham số (ví dụ: orderId không hợp lệ), trả về lỗi với thông điệp chi tiết
                return BadRequest(new { message = $"Invalid parameter: {argEx.Message}" });
            }
            catch (Exception ex)
            {
                // Xử lý tất cả các lỗi khác, trả về lỗi server với thông tin chi tiết
                return StatusCode(500, new { message = "An error occurred while processing your request.", details = ex.Message });
            }
        }
        
        [HttpPost("create-order-customer")]
        public async Task<IActionResult> CreateOrder([FromBody] OrderCreateDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _orderService.CreateOrderAsync(request);
            return Ok(new { message = result });
        }


        //Pham Son Tung
        [HttpGet("get-order-general-data/{orderId}")]
        public async Task<IActionResult> GetOrderGeneralDataPosAsync([FromRoute] int orderId)
        {
            try
            {
                var result = await _orderService.GetOrderGeneralDataPosAsync(orderId);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = $"Order not found: {ex.Message}"
                });
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(500, new
                {
                    message = "A database query error occurred while retrieving order data.",
                    detail = dbEx.InnerException?.Message ?? dbEx.Message
                });
            }
            catch (InvalidOperationException invalidEx)
            {
                return StatusCode(500, new
                {
                    message = "An invalid operation occurred while retrieving order data.",
                    detail = invalidEx.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An unexpected error occurred while processing the request.",
                    detail = ex.Message
                });
            }
        }

        //Pham Son Tung
        [HttpDelete("delete-order-detail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteOrderDetail([FromQuery] string? orderId, [FromQuery] string? orderDetailId)
        {
            try
            {
                // Validate query parameters
                if (string.IsNullOrWhiteSpace(orderId) || !int.TryParse(orderId, out int orderIdParsed))
                {
                    return BadRequest(new { message = "Invalid 'orderId' parameter." });
                }

                if (string.IsNullOrWhiteSpace(orderDetailId) || !int.TryParse(orderDetailId, out int orderDetailIdParsed))
                {
                    return BadRequest(new { message = "Invalid 'orderDetailId' parameter." });
                }

                // Call service to delete
                await _orderService.DeleteOrderDetailAsync(orderIdParsed, orderDetailIdParsed);

                return Ok(new { message = "Order detail deleted successfully." });
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
                return StatusCode(500, new
                {
                    message = "Failed to delete order detail. The item may have already been processed or a database error occurred.",
                    error = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An unexpected error occurred while deleting the order detail.",
                    error = ex.Message
                });
            }
        }

        //Pham Son Tung
        [HttpDelete("delete-confirmed-order-detail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteConfirmedOrderDetail(
            [FromQuery] int? orderId,
            [FromQuery] int? orderDetailId,
            [FromQuery] int? cashierId,
            [FromQuery] string? reason)
        {
            try
            {
                // Validate parameters
                if (orderId is null || orderId <= 0)
                    return BadRequest(new { message = "Parameter 'orderId' is required and must be a positive integer." });

                if (orderDetailId is null || orderDetailId <= 0)
                    return BadRequest(new { message = "Parameter 'orderDetailId' is required and must be a positive integer." });

                if (cashierId is null || cashierId <= 0)
                    return BadRequest(new { message = "Parameter 'cashierId' is required and must be a positive integer." });

                if (string.IsNullOrWhiteSpace(reason))
                    return BadRequest(new { message = "Parameter 'reason' is required and must not be empty." });
                var request = new DeleteConfirmedOrderDetailRequestDto
                {
                    OrderId = orderId.Value,
                    OrderDetailId = orderDetailId.Value,
                    CashierId = cashierId.Value,
                    Reason = reason.Trim()
                };
                await _orderService.DeleteConfirmedOrderDetailAsync(request);
                return Ok(new { message = "Confirmed order detail was deleted and cancellation logged successfully." });
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
                return StatusCode(500, new { message = "An operation error occurred while deleting the order detail.", error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", error = ex.Message });
            }
        }

        //Phạm Sơn Tùng
        [HttpGet("Get-order-payment-information/{orderId}")]
        public async Task<IActionResult> GetOrderPaymentInfo(string orderId)
        {
            try
            {
                // Kiểm tra input: null, rỗng, không phải số
                if (string.IsNullOrWhiteSpace(orderId))
                {
                    return BadRequest("Order ID must not be empty.");
                }

                if (!int.TryParse(orderId, out int parsedOrderId))
                {
                    return BadRequest("Order ID must be a valid integer.");
                }

                // Gọi service
                var orderDto = await _orderService.GetOrderPaymentDtoByIdAsync(parsedOrderId);
                if (orderDto == null)
                {
                    return NotFound($"Order with ID {parsedOrderId} not found.");
                }

                return Ok(orderDto);
            }
            catch (SqlException sqlEx)
            {
                // Lỗi kết nối SQL (truy xuất từ repo)
                return StatusCode(500, $"Database connection error: {sqlEx.Message}");
            }
            catch (DbException dbEx)
            {
                // Lỗi truy vấn CSDL (truy xuất từ repo)
                return StatusCode(500, $"Database query error: {dbEx.Message}");
            }
            catch (Exception ex)
            {
                // Lỗi không xác định (trong service hoặc chung)
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        //Pham Son Tung
        [HttpPost("add-order-delivery-information")]
        public async Task<IActionResult> AddOrderDeliveryInformationAsync([FromBody] DeliveryInformationCreateDto request)
        {
            if (request == null)
            {
                return BadRequest(new { message = "Request body không được null." });
            }
            if (request.OrderId <= 0)
            {
                return BadRequest(new { message = "OrderId không hợp lệ. Phải là số lớn hơn 0." });
            }

            if (string.IsNullOrWhiteSpace(request.ReceiverName))
            {
                return BadRequest(new { message = "ReceiverName không được để trống." });
            }

            if (request.ReceiverName.Length > 100)
            {
                return BadRequest(new { message = "ReceiverName không được vượt quá 100 ký tự." });
            }

            if (string.IsNullOrWhiteSpace(request.ReceiverPhone))
            {
                return BadRequest(new { message = "ReceiverPhone không được để trống." });
            }

            if (request.ReceiverPhone.Length > 20)
            {
                return BadRequest(new { message = "ReceiverPhone không được vượt quá 20 ký tự." });
            }

            if (string.IsNullOrWhiteSpace(request.ReceiverAddress))
            {
                return BadRequest(new { message = "ReceiverAddress không được để trống." });
            }

            if (request.ReceiverAddress.Length > 255)
            {
                return BadRequest(new { message = "ReceiverAddress không được vượt quá 255 ký tự." });
            }
            if (!string.IsNullOrEmpty(request.DeliveryNote) && request.DeliveryNote.Length > 255)
            {
                return BadRequest(new { message = "ReceiverAddress không được vượt quá 255 ký tự." });
            }

            try
            {
                var success = await _orderService.CreateDeliveryInformationServiceAsync(request);

                if (success)
                {
                    return Ok(new { message = "Tạo thông tin giao hàng thành công." });
                }
                else
                {
                    return StatusCode(500, new { message = "Tạo thông tin giao hàng thất bại." });
                }
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new { message = "Lỗi khi thao tác với cơ sở dữ liệu.", error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi không xác định.", error = ex.Message });
            }
        }

        //Phạm Sơn Tùng
        [HttpGet("delivery-information/{orderId}")]
        public async Task<IActionResult> GetDeliveryInformationByOrderId(int orderId)
        {
            if (orderId <= 0)
            {
                return BadRequest(new { message = "OrderId không hợp lệ. Phải là số lớn hơn 0." });
            }

            try
            {
                var deliveryInfoDto = await _orderService.GetDeliveryInformationByOrderIdAsync(orderId);

                if (deliveryInfoDto == null)
                {
                    return NotFound(new { message = $"Không tìm thấy thông tin giao hàng cho order ID {orderId}." });
                }

                return Ok(new
                {
                    message = "Lấy thông tin giao hàng thành công.",
                    data = deliveryInfoDto
                });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(500, new { message = "Lỗi khi thao tác với cơ sở dữ liệu.", error = dbEx.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi không xác định.", error = ex.Message });
            }
        }




    }
}