using BepKhoiBackend.DataAccess.Abstract.OrderAbstract;
using BepKhoiBackend.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace BepKhoiBackend.DataAccess.Repository.OrderRepository
{
    public class OrderRepository : IOrderRepository
    {
        public readonly bepkhoiContext _context;

        public OrderRepository(bepkhoiContext context)
        {
            _context = context;
        }

        //Task for Create Order
        public async Task<Order> CreateOrderPosAsync(Order order)
        {
            try
            {
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
                return order;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while saving the order to database.", ex);
            }
        }

        // Get order by Id for Pos site
        public async Task<Order?> GetOrderByIdPosAsync(int orderId)
        {
            return await _context.Orders
                    .AsNoTracking()
                    .FirstOrDefaultAsync(o => o.OrderId == orderId);
        }

        // Function to update order for Pos site
        public async Task<Order> UpdateOrderAsyncPos(Order order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
            return order;
        }

        // Function to get Order Detail by Id for pos site
        public async Task<OrderDetail> GetOrderDetailByIdAsync(int orderDetailId)
        {
            return await _context.OrderDetails.AsNoTracking().FirstOrDefaultAsync(od => od.OrderDetailId == orderDetailId);
        }

        // Function to Update Order Detail 
        public async Task UpdateOrderDetailAsync(OrderDetail orderDetail)
        {
            _context.OrderDetails.Update(orderDetail);
            await _context.SaveChangesAsync();
        }

        // Func to get customer by Id
        public async Task<Customer?> GetCustomerByIdAsync(int customerId)
        {
            return await _context.Customers
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);
        }

        // Func to update order
        public async Task UpdateOrderAsync(Order order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }

        // Func to get product by Id
        public async Task<Menu?> GetProductByIdAsync(int productId)
        {
            return await _context.Menus
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.ProductId == productId);
        }



        //Pham Son Tung
        //Repository function for MoveOrderPos API
        public async Task<bool> MoveOrderPosRepoAsync(int orderId, int orderTypeId, int? roomId, int? userId)
        {
            try
            {
                var order = await _context.Orders.FindAsync(orderId);
                if (order == null)
                {
                    throw new KeyNotFoundException($"Order with ID {orderId} not found.");
                }

                if (orderTypeId != 1 && orderTypeId != 2 && orderTypeId != 3)
                {
                    throw new ArgumentException("Invalid order type. Must be 1 (Takeaway), 2 (Ship), or 3 (Dine-in).");
                }

                if (orderTypeId == 3)
                {
                    var roomExists = await _context.Rooms.AnyAsync(r => r.RoomId == roomId);
                    if (!roomExists)
                    {
                        throw new KeyNotFoundException($"Room with ID {roomId} does not exist.");
                    }
                    order.RoomId = roomId;
                    order.ShipperId = null;
                }
                else
                {
                    order.RoomId = null;
                }

                if (orderTypeId == 2)
                {
                    var userExists = await _context.Users.AnyAsync(u => u.UserId == userId);
                    if (!userExists)
                    {
                        throw new KeyNotFoundException($"User (Shipper) with ID {userId} does not exist.");
                    }
                    order.ShipperId = userId;
                }
                else
                {
                    order.ShipperId = null;
                }

                order.OrderTypeId = orderTypeId;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the order type.", ex);
            }
        }


        //Pham Son Tung
        //Repository Func for CombineOrderPos API
        public async Task<bool> CombineOrderPosRepoAsync(int firstOrderId, int secondOrderId)
        {
            try
            {
                // Kiểm tra xem cả hai đơn hàng có tồn tại không
                var firstOrder = await _context.Orders.FindAsync(firstOrderId);
                var secondOrder = await _context.Orders.FindAsync(secondOrderId);

                if (firstOrder == null)
                {
                    throw new KeyNotFoundException($"Order with ID {firstOrderId} not found.");
                }
                if (secondOrder == null)
                {
                    throw new KeyNotFoundException($"Order with ID {secondOrderId} not found.");
                }

                // Lấy danh sách OrderDetail của đơn hàng thứ nhất
                var orderDetails = await _context.OrderDetails
                                                 .Where(od => od.OrderId == firstOrderId)
                                                 .ToListAsync();

                if (!orderDetails.Any())
                {
                    throw new InvalidOperationException($"Order with ID {firstOrderId} has no order details to transfer.");
                }

                // Cập nhật OrderId của OrderDetail từ firstOrderId sang secondOrderId
                foreach (var detail in orderDetails)
                {
                    detail.OrderId = secondOrderId;
                }

                // Lưu thay đổi vào database
                await _context.SaveChangesAsync();

                return true;
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



    }
}
