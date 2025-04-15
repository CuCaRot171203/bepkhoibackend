using AutoMapper;
using BepKhoiBackend.BusinessObject.Abstract.OrderAbstract;
using BepKhoiBackend.BusinessObject.dtos.CustomerDto;
using BepKhoiBackend.BusinessObject.dtos.MenuDto;
using BepKhoiBackend.BusinessObject.dtos.OrderDetailDto;
using BepKhoiBackend.BusinessObject.dtos.OrderDto;
using BepKhoiBackend.BusinessObject.dtos.OrderDto.PaymentDto;
using BepKhoiBackend.DataAccess.Abstract.OrderAbstract;
using BepKhoiBackend.DataAccess.Abstract.OrderDetailAbstract;
using BepKhoiBackend.DataAccess.Models;
using BepKhoiBackend.Shared.Helpers;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;

namespace BepKhoiBackend.BusinessObject.Services.OrderService
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly IMapper _mapper;

        public OrderService(
            IOrderRepository orderRepository,
            IOrderDetailRepository orderDetailRepository,
            IMapper mapper)
        {
            _orderRepository = orderRepository;
            _orderDetailRepository = orderDetailRepository;
            _mapper = mapper;
        }

        // Method create order
        public async Task<OrderDto> CreateNewOrderAsync(CreateOrderRequestDto request)
        {
            try
            {
                // Validate các trường bắt buộc
                if (request.OrderTypeId <= 0 || request.OrderStatusId <= 0)
                {
                    throw new ArgumentException("OrderTypeId and OrderStatusId are required and must be greater than 0.");
                }

                if (request.OrderStatusId is not (1 or 2 or 3))
                {
                    throw new ArgumentException("OrderStatusId must be either 1, 2, or 3.");
                }

                // Tạo đơn hàng từ DTO
                var newOrder = _mapper.Map<Order>(request);

                switch (request.OrderTypeId)
                {
                    case 3: // Tại bàn
                        if (request.ShipperId != null || request.RoomId == null)
                        {
                            throw new ArgumentException("OrderTypeId = 3 (Tại bàn) requires RoomId and must not have ShipperId.");
                        }

                        var createdTableOrder = await _orderRepository.CreateOrderPosAsync(newOrder);

                        // Cập nhật phòng
                        await _orderRepository.UpdateRoomIsUseByRoomIdAsync(request.RoomId.Value);

                        return _mapper.Map<OrderDto>(createdTableOrder);

                    case 2: // Giao đi
                        if (request.RoomId != null || request.ShipperId == null)
                        {
                            throw new ArgumentException("OrderTypeId = 2 (Giao đi) requires ShipperId and must not have RoomId.");
                        }

                        var createdDeliveryOrder = await _orderRepository.CreateOrderPosAsync(newOrder);
                        return _mapper.Map<OrderDto>(createdDeliveryOrder);

                    case 1: // Tại quầy
                        if (request.RoomId != null || request.ShipperId != null)
                        {
                            throw new ArgumentException("OrderTypeId = 1 (Tại quầy) must not have RoomId or ShipperId.");
                        }

                        var createdCounterOrder = await _orderRepository.CreateOrderPosAsync(newOrder);
                        return _mapper.Map<OrderDto>(createdCounterOrder);

                    default:
                        throw new ArgumentException("Invalid OrderTypeId. Must be 1, 2, or 3.");
                }
            }
            catch (Exception ex)
            {
                // Có thể log lại nếu muốn
                throw new Exception($"Tạo đơn hàng thất bại: {ex.Message}", ex);
            }
        }



        // Method add note to order async
        public async Task<OrderDto> AddOrderNoteToOrderPosAsync(AddNoteRequest request)
        {
            var order = await _orderRepository.GetOrderByIdPosAsync(request.OrderId);
            //check null
            if (order == null)
            {
                throw new ArgumentException($"Order with Id {request.OrderId} not found.");
            }

            order.OrderNote = request.OrderNote;
            //var updatedOrder = await _orderRepository.UpdateOrderAsyncPos(order);

            var result = _mapper.Map<OrderDto>(await _orderRepository.UpdateOrderAsyncPos(order));

            return result;
        }

        // Method update order detail quantity pos
        public async Task<OrderDetailDto> UpdateOrderDetailQuantiyPosAsync(UpdateOrderDetailQuantityRequest request)
        {
            // 1. Lấy dữ liệu OrderDetail
            var orderDetail = await _orderRepository.GetOrderDetailByIdAsync(request.OrderDetailId);
            if (orderDetail == null)
            {
                throw new KeyNotFoundException("Order detail not found.");
            }

            // 2. Ưu tiên xử lý theo Quantity nếu có
            if (request.Quantity.HasValue)
            {
                if (request.Quantity <= 0)
                {
                    throw new ArgumentException("Quantity must be greater than 0.");
                }

                orderDetail.Quantity = request.Quantity.Value;
            }
            // 3. Nếu không có Quantity thì xét IsAdd
            else if (request.IsAdd.HasValue)
            {
                if (request.IsAdd.Value)
                {
                    orderDetail.Quantity += 1;
                }
                else
                {
                    if (orderDetail.Quantity <= 1)
                    {
                        throw new ArgumentException("Quantity cannot be reduced to 0.");
                    }

                    orderDetail.Quantity -= 1;
                }
            }
            // 4. Nếu không có cả Quantity và IsAdd thì báo lỗi
            else
            {
                throw new ArgumentException("Either Quantity or IsAdd must be provided.");
            }

            // 5. Lưu thay đổi
            await _orderRepository.UpdateOrderDetailAsync(orderDetail);

            // 6. Cập nhật lại tổng số lượng & tổng tiền của order
            await _orderRepository.UpdateOrderAfterAddOrderDetailAsync(orderDetail.OrderId);

            // 7. Trả về kết quả DTO
            return new OrderDetailDto
            {
                OrderDetailId = orderDetail.OrderDetailId,
                OrderId = orderDetail.OrderId,
                ProductId = orderDetail.ProductId,
                ProductName = orderDetail.ProductName,
                Quantity = orderDetail.Quantity,
                Price = orderDetail.Price,
                ProductNote = orderDetail.ProductNote,
            };
        }


        // Method to add customer to order 
        public async Task<bool> AddCustomerToOrderAsync(AddCustomerToOrderRequest request)
        {
            try
            {
                var order = await _orderRepository.GetOrderByIdPosAsync(request.OrderId);
                if (order == null)
                {
                    throw new KeyNotFoundException($"Order with ID {request.OrderId} not found.");
                }

                var customer = await _orderRepository.GetCustomerByIdAsync(request.CustomerId);
                if (customer == null || (customer.IsDelete.HasValue && customer.IsDelete.Value))
                {
                    throw new KeyNotFoundException($"Customer with ID {request.CustomerId} not found or has been deleted.");
                }

                if (order.CustomerId == request.CustomerId)
                {
                    throw new ArgumentException("This customer is already assigned to the order.");
                }
                order.CustomerId = request.CustomerId;

                await _orderRepository.UpdateOrderAsync(order);
                return true;
            }
            catch
            {
                throw;
            }
        }

        // Method to Add product to order
        public async Task<bool> AddProductToOrderAsync(AddProductToOrderRequest request)
        {
            try
            {
                var order = await _orderRepository.GetOrderByIdPosAsync(request.OrderId);
                if (order == null)
                {
                    throw new KeyNotFoundException($"Order with ID {request.OrderId} not found.");
                }

                var product = await _orderRepository.GetProductByIdAsync(request.ProductId);
                if (product == null)
                {
                    throw new KeyNotFoundException($"Product with ID {request.ProductId} not found.");
                }

                var existingOrderDetail = await _orderDetailRepository
                    .GetOrderDetailByProductAsync(request.OrderId, request.ProductId);

                if (existingOrderDetail != null)
                {
                    // Repo đã đảm bảo là ProductNote == null và Status == false
                    existingOrderDetail.Quantity += 1;
                    await _orderDetailRepository.UpdateOrderDetailAsync(existingOrderDetail);
                }
                else
                {
                    // Nếu không tìm thấy, thêm mới sản phẩm
                    var newOrderDetail = new OrderDetail
                    {
                        OrderId = request.OrderId,
                        ProductId = request.ProductId,
                        ProductName = product.ProductName,
                        Quantity = 1,
                        Price = product.SellPrice,
                        ProductNote = null,
                        Status = false
                    };

                    await _orderDetailRepository.AddOrderDetailAsync(newOrderDetail);
                }
                await _orderRepository.UpdateOrderAfterAddOrderDetailAsync(request.OrderId);
                return true;
            }
            catch
            {
                throw;
            }
        }



        //Pham Son Tung
        //Service Func for MoveOrderPos API
        public async Task<bool> ChangeOrderTypeServiceAsync(MoveOrderPosRequestDto request)
        {
            // Kiểm tra điều kiện hợp lệ trước khi gọi repository
            if (request.OrderTypeId == 1) // Đơn mang về
            {
                if (request.RoomId != null || request.UserId != null)
                {
                    throw new ArgumentException("Invalid parameter. Takeaway order cannot have RoomId or UserId.");
                }
            }
            else if (request.OrderTypeId == 2) // Đơn ship
            {
                if (request.RoomId != null)
                {
                    throw new ArgumentException("Invalid parameter. Ship order cannot have RoomId.");
                }
                if (request.UserId == null)
                {
                    throw new ArgumentException("Ship order must have a ShipperId.");
                }
            }
            else if (request.OrderTypeId == 3) // Đơn ăn tại quán
            {
                if (request.UserId != null)
                {
                    throw new ArgumentException("Invalid parameter. Dine-in order cannot have UserId.");
                }
                if (request.RoomId == null)
                {
                    throw new ArgumentException("Dine-in order must have RoomId.");
                }
            }
            else
            {
                throw new ArgumentException("Invalid OrderTypeId. Must be 1 (Takeaway), 2 (Ship), or 3 (Dine-in).");
            }

            // Gọi repository để cập nhật dữ liệu
            return await _orderRepository.MoveOrderPosRepoAsync(
            request.OrderId, request.OrderTypeId, request.RoomId, request.UserId);
        }

        //Pham Son Tung
        //Service function for CombineOrderPos API
        public async Task<bool> CombineOrderPosServiceAsync(CombineOrderPosRequestDto request)
        {
            try
            {
                // Kiểm tra nếu orderId là null
                if (request.FirstOrderId == null || request.SecondOrderId == null)
                {
                    throw new ArgumentNullException("Order ID cannot be null.");
                }

                // Kiểm tra nếu orderId không hợp lệ
                if (request.FirstOrderId <= 0 || request.SecondOrderId <= 0)
                {
                    throw new ArgumentException("Order ID must be a positive integer.");
                }

                // Gọi repository để thực hiện việc chuyển order details
                return await _orderRepository.CombineOrderPosRepoAsync(request.FirstOrderId.Value, request.SecondOrderId.Value);
            }
            catch (ArgumentNullException ex)
            {
                throw new ArgumentNullException("Invalid input: Order ID cannot be null.", ex);
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException("Invalid input: Order ID must be a positive integer.", ex);
            }
            catch (KeyNotFoundException ex)
            {
                throw new KeyNotFoundException("Resource not found.", ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException("Business logic error.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while combining orders.", ex);
            }
        }

        //Pham Son Tung
        public async Task<IEnumerable<OrderDtoPos>> GetOrdersByTypePosAsync(int? roomId, int? shipperId, int? orderTypeId)
        {
            try
            {
                // Gọi hàm repository để lấy danh sách đơn hàng theo kiểu
                var orders = await _orderRepository.GetOrdersByTypePos(roomId, shipperId, orderTypeId);

                // Chuyển đổi từ Order sang OrderDtoPos
                var orderDtos = orders.Select(o => new OrderDtoPos
                {
                    OrderId = o.OrderId,
                    CustomerId = o.CustomerId,
                    ShipperId = o.ShipperId,
                    DeliveryInformationId = o.DeliveryInformationId,
                    OrderTypeId = o.OrderTypeId,
                    RoomId = o.RoomId,
                    CreatedTime = o.CreatedTime,
                    TotalQuantity = o.TotalQuantity,
                    AmountDue = o.AmountDue,
                    OrderStatusId = o.OrderStatusId,
                    OrderNote = o.OrderNote,
                }).ToList();
                return orderDtos;
            }
            catch (ArgumentException argEx)
            {
                // Log lỗi và ném lại cho API xử lý
                throw new Exception($"Invalid parameter: {argEx.Message}", argEx);
            }
            catch (Exception ex)
            {
                // Log tất cả các lỗi khác
                throw new Exception("An error occurred while processing your request.", ex);
            }
        }

        //Pham Son Tung
        public async Task<CustomerPosDto> GetCustomerIdByOrderIdAsync(int orderId)
        {
            try
            {
                var customer = await _orderRepository.GetCustomerIdByOrderIdAsync(orderId);

                var customerPosDto = new CustomerPosDto
                {
                    customerId = customer.CustomerId,
                    customerName = customer.CustomerName,
                    phone = customer.Phone
                };

                return customerPosDto;
            }
            catch (KeyNotFoundException ex)
            {
                // Có thể log lại và ném ra lại cho Controller xử lý
                throw new KeyNotFoundException($"Order with ID {orderId} not found at service.", ex);
            }
            catch (Exception ex)
            {
                // Logging hoặc xử lý thêm nếu cần
                throw new Exception("An error occurred in OrderService while retrieving the customer.", ex);
            }
        }

        //Pham Son Tung
        public async Task AssignCustomerToOrderAsync(int orderId, int customerId)
        {
            try
            {
                await _orderRepository.AssignCustomerToOrder(orderId, customerId);
            }
            catch (KeyNotFoundException ex)
            {
                // Ghi log hoặc xử lý lỗi nếu cần
                throw new Exception($"Order or Customer not found. {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                // Logging hoặc xử lý lỗi khác nếu cần
                throw new Exception("An error occurred in OrderService while assigning customer to order.", ex);
            }
        }

        //pham son tung
        public async Task<bool> RemoveCustomerFromOrderAsync(int orderId)
        {
            try
            {
                // Gọi phương thức từ repository để xóa CustomerId
                var result = await _orderRepository.RemoveCustomerFromOrderAsync(orderId);
                // Xử lý logic sau khi xóa (nếu cần, như log lỗi, thông báo thành công,...)
                if (result)
                {
                    // Thành công
                    return true;
                }
                // Nếu không tìm thấy đơn hàng hoặc gặp lỗi
                return false;
            }
            catch (Exception ex)
            {
                // Xử lý lỗi xảy ra trong service
                Console.Error.WriteLine($"Service error: {ex.Message}");
                return false;
            }
        }

        //pham son tung
        public async Task<bool> RemoveOrderById(int orderId)
        {
            try
            {
                // Gọi hàm từ repository để thực hiện "xóa mềm"
                var removedOrder = await _orderRepository.RemoveOrder(orderId);
                // Nếu có RoomId thì cập nhật lại trạng thái phòng
                if (removedOrder.RoomId.HasValue)
                {
                    await _orderRepository.UpdateRoomIsUseByRoomIdAsync(removedOrder.RoomId.Value);
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in RemoveOrderAsync: {ex.Message}", ex);
            }
        }

        //Pham Son Tung
        public async Task<IEnumerable<OrderDetailDtoPos>> GetOrderDetailsByOrderIdAsync(int orderId)
        {
            try
            {
                var orderDetails = await _orderRepository.GetOrderDetailsByOrderIdAsync(orderId);
                var detailDtos = orderDetails.Select(od => new OrderDetailDtoPos
                {
                    OrderDetailId = od.OrderDetailId,
                    OrderId = od.OrderId,
                    Status = od.Status,
                    ProductId = od.ProductId,
                    ProductName = od.ProductName,
                    Quantity = od.Quantity,
                    Price = od.Price,
                    ProductNote = od.ProductNote
                });

                return detailDtos;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving order details.", ex);
            }
        }

        public async Task<ResultWithList<OrderDto>> GetAllOrdersAsync()
        {
            try
            {
                var orders = await _orderRepository.GetAllAsync();

                var data = orders.Select(o => new OrderDto
                {
                    OrderId = o.OrderId,
                    CustomerId = o.CustomerId,
                    ShipperId = o.ShipperId,
                    DeliveryInformationId = o.DeliveryInformationId,
                    OrderTypeId = o.OrderTypeId,
                    RoomId = o.RoomId,
                    CreatedTime = o.CreatedTime,
                    TotalQuantity = o.TotalQuantity,
                    AmountDue = o.AmountDue,
                    OrderStatusId = o.OrderStatusId,
                    OrderNote = o.OrderNote
                }).ToList();

                return new ResultWithList<OrderDto>
                {
                    IsSuccess = true,
                    Message = "Fetched all orders successfully.",
                    Data = data
                };
            }
            catch (Exception ex)
            {
                return new ResultWithList<OrderDto>
                {
                    IsSuccess = false,
                    Message = $"Error: {ex.Message}"
                };
            }
        }

        public async Task<ResultWithList<OrderDto>> FilterOrdersByDateAsync(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var orders = await _orderRepository.GetByDateRangeAsync(fromDate, toDate);

                var data = orders.Select(o => new OrderDto
                {
                    OrderId = o.OrderId,
                    CustomerId = o.CustomerId,
                    ShipperId = o.ShipperId,
                    DeliveryInformationId = o.DeliveryInformationId,
                    OrderTypeId = o.OrderTypeId,
                    RoomId = o.RoomId,
                    CreatedTime = o.CreatedTime,
                    TotalQuantity = o.TotalQuantity,
                    AmountDue = o.AmountDue,
                    OrderStatusId = o.OrderStatusId,
                    OrderNote = o.OrderNote
                }).ToList();

                return new ResultWithList<OrderDto>
                {
                    IsSuccess = true,
                    Message = "Filtered orders successfully.",
                    Data = data
                };
            }
            catch (Exception ex)
            {
                return new ResultWithList<OrderDto>
                {
                    IsSuccess = false,
                    Message = $"Error: {ex.Message}"
                };
            }
        }


        public async Task<string> CreateOrderAsync(OrderCreateDTO dto)
        {
            // 1) Map DTO → Entity Order
            var order = new Order
            {
                CustomerId = dto.CustomerId,
                OrderTypeId = dto.OrderTypeId,
                RoomId = dto.RoomId,
                CreatedTime = DateTime.UtcNow,
                TotalQuantity = dto.TotalQuantity,
                AmountDue = dto.AmountDue,
                OrderStatusId = dto.OrderStatusId,
                OrderNote = dto.OrderNote,
            };

            // 2) Gán chi tiết
            order.OrderDetails = dto.OrderDetails
                .Select(d => new OrderDetail
                {
                    ProductId = d.ProductId,
                    ProductName = d.ProductName,
                    Quantity = d.Quantity,
                    Price = d.Price,
                    ProductNote = d.ProductNote,
                    Status = d.Status
                })
                .ToList();

            // 3) Lưu 1 lần, EF tự gán OrderId cho từng detail
            try
            {
                await _orderRepository.AddOrderAsync(order);
            }
            catch (DbUpdateException ex)
            {
                // Bắt lỗi để xem InnerException
                throw new InvalidOperationException(
                    "Lỗi khi lưu Order/OrderDetail: " + ex.InnerException?.Message, ex);
            }

            return $"Order {order.OrderId} created with {order.OrderDetails.Count} items.";
        }



        //Pham son tung
        public async Task<OrderGeneralDataPosDto> GetOrderGeneralDataPosAsync(int orderId)
        {
            var order = await _orderRepository.GetAllOrderData(orderId);

            var hasUnconfirmProducts = order.OrderDetails != null &&
                                       order.OrderDetails.Any(od => od.Status == false);

            return new OrderGeneralDataPosDto
            {
                OrderId = order.OrderId,
                OrderNote = order.OrderNote,
                TotalQuantity = order.TotalQuantity,
                AmountDue = order.AmountDue,
                HasUnconfirmProducts = hasUnconfirmProducts
            };
        }

        //Pham Son Tung
        public async Task DeleteOrderDetailAsync(int orderId, int orderDetailId)
        {
            // 1. Validate input
            if (orderId <= 0)
            {
                throw new ArgumentException("OrderId must be greater than 0.");
            }

            if (orderDetailId <= 0)
            {
                throw new ArgumentException("OrderDetailId must be greater than 0.");
            }

            try
            {
                // 2. Call repository to delete
                await _orderRepository.DeleteOrderDetailAsync(orderId, orderDetailId);
            }
            catch (InvalidOperationException ex)
            {
                // Can be due to Status == true or DB update issue
                throw new InvalidOperationException("Failed to delete order detail. The item may have already been processed or a database error occurred.", ex);
            }
            catch (KeyNotFoundException ex)
            {
                // If OrderDetail is not found
                throw new KeyNotFoundException("Order detail not found for deletion.", ex);
            }
            catch (Exception)
            {
                // Rethrow unexpected errors
                throw;
            }
        }
        public async Task DeleteConfirmedOrderDetailAsync(DeleteConfirmedOrderDetailRequestDto request)
        {
            // 1. Kiểm tra đầu vào
            if (request.OrderId <= 0)
                throw new ArgumentException("OrderId must be greater than 0.");
            if (request.OrderDetailId <= 0)
                throw new ArgumentException("OrderDetailId must be greater than 0.");
            if (request.CashierId <= 0)
                throw new ArgumentException("CashierId must be greater than 0.");
            if (string.IsNullOrWhiteSpace(request.Reason))
                throw new ArgumentException("Reason must not be empty.");
            try
            {
                // 2. Mapping sang entity OrderCancellationHistory
                var cancellation = new OrderCancellationHistory
                {
                    CashierId = request.CashierId,
                    Reason = request.Reason
                    // OrderId, ProductId, Quantity sẽ được gán trong repo
                };

                // 3. Gọi repository để thực hiện xoá có lưu lịch sử
                await _orderRepository.DeleteConfirmedOrderDetailAsync(request.OrderId, request.OrderDetailId, cancellation);
            }
            catch (KeyNotFoundException ex)
            {
                throw new KeyNotFoundException("OrderDetail not found.", ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException("Failed to delete confirmed OrderDetail.", ex);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<OrderPaymentDto?> GetOrderPaymentDtoByIdAsync(int orderId)
        {
            try
            {
                var order = await _orderRepository.GetFullOrderByIdAsync(orderId);
                if (order == null) return null;

                var orderDto = new OrderPaymentDto
                {
                    OrderId = order.OrderId,
                    CustomerId = order.CustomerId,
                    ShipperId = order.ShipperId,
                    DeliveryInformationId = order.DeliveryInformationId,
                    OrderTypeId = order.OrderTypeId,
                    RoomId = order.RoomId,
                    CreatedTime = order.CreatedTime,
                    TotalQuantity = order.TotalQuantity,
                    AmountDue = order.AmountDue,
                    OrderStatusId = order.OrderStatusId,
                    OrderNote = order.OrderNote,
                    Customer = order.Customer != null
                        ? new CustomerPaymertDto
                        {
                            CustomerId = order.Customer.CustomerId,
                            Phone = order.Customer.Phone,
                            CustomerName = order.Customer.CustomerName
                        }
                        : null,
                    OrderDetails = order.OrderDetails?.Select(od => new OrderDetailPaymentDto
                    {
                        OrderDetailId = od.OrderDetailId,
                        OrderId = od.OrderId,
                        Status = od.Status,
                        ProductId = od.ProductId,
                        ProductName = od.ProductName,
                        Quantity = od.Quantity,
                        Price = od.Price,
                        ProductNote = od.ProductNote,
                        ProductVat = od.Product?.ProductVat ?? 0 // tránh null
                    }).ToList()
                };

                return orderDto;
            }
            catch (SqlException sqlEx)
            {
                throw;
            }
            catch (DbException dbEx)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception($"Service error when fetching order: {ex.Message}", ex);
            }
        }



    }
}
