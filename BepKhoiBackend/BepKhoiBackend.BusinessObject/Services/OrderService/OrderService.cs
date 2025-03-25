using AutoMapper;
using BepKhoiBackend.BusinessObject.Abstract.OrderAbstract;
using BepKhoiBackend.BusinessObject.dtos.CustomerDto;
using BepKhoiBackend.BusinessObject.dtos.MenuDto;
using BepKhoiBackend.BusinessObject.dtos.OrderDetailDto;
using BepKhoiBackend.BusinessObject.dtos.OrderDto;
using BepKhoiBackend.DataAccess.Abstract.OrderAbstract;
using BepKhoiBackend.DataAccess.Abstract.OrderDetailAbstract;
using BepKhoiBackend.DataAccess.Models;

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
        public async Task<OrderDto> CreateNewOrderAsync(CreateOrderRequest request)
        {
            if (request.OrderTypeId <= 0 || request.OrderStatusId <= 0)
            {
                throw new ArgumentException("OrderTypeId and OrderStatusId are required.");
            }

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


    }
}
