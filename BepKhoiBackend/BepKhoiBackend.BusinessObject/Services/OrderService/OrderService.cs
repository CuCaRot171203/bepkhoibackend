using AutoMapper;
using BepKhoiBackend.BusinessObject.Abstract.OrderAbstract;
using BepKhoiBackend.BusinessObject.dtos.CustomerDto;
using BepKhoiBackend.BusinessObject.dtos.MenuDto;
using BepKhoiBackend.BusinessObject.dtos.OrderDetailDto;
using BepKhoiBackend.BusinessObject.dtos.OrderDto;
using BepKhoiBackend.DataAccess.Abstract.OrderAbstract;
using BepKhoiBackend.DataAccess.Abstract.OrderDetailAbstract;
using BepKhoiBackend.DataAccess.Models;
using Org.BouncyCastle.Asn1.Ocsp;

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
            // Kiểm tra OrderTypeId và OrderStatusId không null hoặc <= 0
            if (request.OrderTypeId <= 0 || request.OrderStatusId <= 0)
            {
                throw new ArgumentException("OrderTypeId and OrderStatusId are required and must be greater than 0.");
            }

            // Kiểm tra OrderStatusId chỉ được là 1, 2 hoặc 3
            if (request.OrderStatusId is not (1 or 2 or 3))
            {
                throw new ArgumentException("OrderStatusId must be either 1, 2, or 3.");
            }

            // Kiểm tra OrderTypeId chỉ được là 1, 2 hoặc 3 và áp logic riêng
            switch (request.OrderTypeId)
            {
                case 3: // Tại bàn
                    if (request.ShipperId != null || request.RoomId == null)
                    {
                        throw new ArgumentException("OrderTypeId = 3 (Tại bàn) requires RoomId and must not have ShipperId.");
                    }
                    break;

                case 2: // Mang về
                    if (request.RoomId != null || request.ShipperId == null)
                    {
                        throw new ArgumentException("OrderTypeId = 2 requires ShipperId and must not have RoomId.");
                    }
                    break;

                case 1: // Mua tại quầy
                    if (request.RoomId != null || request.ShipperId != null)
                    {
                        throw new ArgumentException("OrderTypeId = 1 requires must not have RoomId and must not have ShipperId.");
                    }
                    break;

                default:
                    throw new ArgumentException("Invalid OrderTypeId. Must be 1, 2, or 3.");
            }

            // Mapping và tạo đơn
            var newOrder = _mapper.Map<Order>(request);
            var createdOrder = await _orderRepository.CreateOrderPosAsync(newOrder);

            return _mapper.Map<OrderDto>(createdOrder);
        }


        // Method add note to order async
        public async Task<OrderDto> AddOrderNoteToOrderPosAsync(AddNoteRequest request)
        {
            var order = await _orderRepository.GetOrderByIdPosAsync(request.OrderId);
            //check null
            if(order == null)
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
            var orderDetail = await _orderRepository.GetOrderDetailByIdAsync(request.OrderDetailId);

            // check null
            if(orderDetail == null)
            {
                throw new KeyNotFoundException("Order detail not found.");
            }

            // Check quantity 
            if (request.Quantity.HasValue)
            {
                if(request.Quantity <= 0)
                {
                    throw new ArgumentException("Quantity must be greater than 0.");
                }

                orderDetail.Quantity = request.Quantity.Value;
            }
            else if (request.Quantity.HasValue)
            {
                if (request.IsAdd.Value)
                {
                    orderDetail.Quantity += 1;
                }
                else
                {
                    if(orderDetail.Quantity > 1)
                    {
                        orderDetail.Quantity -= 1;
                    }
                    else
                    {
                        throw new ArgumentException("Quantity cannot be reduce to 0.");
                    }
                }
            }
            else
            {
                throw new ArgumentException("Quantity or IsAdd must be provided.");
            }

            await _orderRepository.UpdateOrderDetailAsync(orderDetail);

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

                // check null
                if(order == null)
                {
                    throw new KeyNotFoundException($"Order with ID {request.OrderId} not found.");
                }

                var product = await _orderRepository.GetProductByIdAsync(request.ProductId);

                // Check null
                if (product == null)
                {
                    throw new KeyNotFoundException($"Product with ID {request.ProductId} not found.");
                }

                var existingOrderDetail = await _orderDetailRepository.GetOrderDetailByProductAsync(request.OrderId, request.ProductId);
                if (existingOrderDetail != null && string.IsNullOrEmpty(existingOrderDetail.ProductNote))
                {
                    // if OrderDetail exist and not have note, increase value quantity
                    existingOrderDetail.Quantity += 1;
                    await _orderDetailRepository.UpdateOrderDetailAsync(existingOrderDetail);
                }
                else
                {
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
                    // Chuyển đổi OrderDetails sang OrderDetailDtoPos
                    OrderDetails = o.OrderDetails.Select(od => new OrderDetailDtoPos
                    {
                        OrderDetailId = od.OrderDetailId,
                        OrderId = od.OrderId,
                        Status = od.Status,
                        ProductId = od.ProductId,
                        ProductName = od.ProductName,
                        Quantity = od.Quantity,
                        Price = od.Price,
                        ProductNote = od.ProductNote
                    }).ToList()
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
    }
}
