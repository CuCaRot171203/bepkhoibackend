using BepKhoiBackend.DataAccess.Abstract.OrderAbstract;
using BepKhoiBackend.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

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

        //Pham Son Tung
        public async Task<IEnumerable<Order>> GetOrdersByTypePos(int? roomId, int? shipperId, int? orderTypeId)
        {
            if (!orderTypeId.HasValue)
                throw new ArgumentException("orderTypeId is required.");

            IQueryable<Order> query = _context.Orders
                .AsNoTracking();

            try
            {
                switch (orderTypeId)
                {
                    case 1:
                        query = query.Where(o => o.OrderStatusId == 1);
                        break;

                    case 2:
                        if (!shipperId.HasValue)
                            throw new ArgumentException("shipperId is required for orderTypeId = 2.");
                        query = query.Where(o => o.OrderStatusId == 1 && o.ShipperId == shipperId.Value);
                        break;

                    case 3:
                        if (!roomId.HasValue)
                            throw new ArgumentException("roomId is required for orderTypeId = 3.");
                        query = query.Where(o => o.OrderStatusId == 1 && o.RoomId == roomId.Value);
                        break;

                    default:
                        throw new ArgumentException("Invalid orderTypeId. Accepted values: {1, 2, 3}.");
                }

                // Thực hiện truy vấn và trả về kết quả
                return await query.ToListAsync();
            }
            catch (DbUpdateException dbEx)
            {
                // Bắt lỗi liên quan đến cập nhật database
                throw new Exception("Database update error occurred while fetching orders.", dbEx);
            }
            catch (Exception ex)
            {
                // Bắt tất cả các lỗi khác và ném lỗi tùy chỉnh
                throw new Exception("An error occurred while fetching orders.", ex);
            }
        }

        //-------------NgocQuan---------------//
        public async Task<Order?> GetOrderWithDetailsAsync(int orderId)
        {
            return await _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);
        }
        //Pham Son Tung
        public async Task<Customer> GetCustomerIdByOrderIdAsync(int orderId)
        {
            try
            {
                var order = await _context.Orders
                    .FirstOrDefaultAsync(o => o.OrderId == orderId);

                if (order == null)
                {
                    throw new KeyNotFoundException($"Order with ID {orderId} was not found at repository.");
                }
                var customer = await _context.Customers.FirstOrDefaultAsync(c=>c.CustomerId == order.CustomerId);
                if (customer == null)
                {
                    throw new KeyNotFoundException($"customer with ID {order.Customer} was not found at repository.");
                }
                return customer;
            }
            catch (DbUpdateException dbEx)
            {
                // Lỗi liên quan đến thao tác database (transaction, connection...)
                throw new Exception("Database update error occurred while fetching the order.", dbEx);
            }
            catch (DbException dbEx)
            {
                // Các lỗi database khác (nếu dùng System.Data.Common)
                throw new Exception("A database error occurred while retrieving the order.", dbEx);
            }
            catch (Exception ex)
            {
                // Catch all để tránh app crash
                throw new Exception("An unexpected error occurred while getting the customer ID.", ex);
            }
        }

        public async Task AssignCustomerToOrder(int orderId, int customerId)
        {
            try
            {
                var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);
                if (order == null)
                {
                    throw new KeyNotFoundException($"Order with ID {orderId} was not found.");
                }

                var customer = await _context.Customers.FirstOrDefaultAsync(c => c.CustomerId == customerId);
                if (customer == null)
                {
                    throw new KeyNotFoundException($"Customer with ID {customerId} was not found.");
                }

                order.CustomerId = customerId;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException dbEx)
            {
                throw new Exception("Database update error occurred while assigning customer to order.", dbEx);
            }
            catch (DbException dbEx)
            {
                throw new Exception("A database error occurred while assigning customer to order.", dbEx);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while assigning customer to order.", ex);
            }
        }

        //Pham Son Tung
        public async Task<bool> RemoveCustomerFromOrderAsync(int orderId)
        {
            try
            {
                // Tìm đơn hàng với orderId
                var order = await _context.Orders
                                          .FirstOrDefaultAsync(o => o.OrderId == orderId);

                // Kiểm tra xem đơn hàng có tồn tại không
                if (order == null)
                {
                    return false; // Không tìm thấy đơn hàng
                }

                // Set CustomerId về null
                order.CustomerId = null;

                // Lưu thay đổi vào cơ sở dữ liệu
                await _context.SaveChangesAsync();

                return true; // Thành công
            }
            catch (DbUpdateException dbEx)
            {
                // Xử lý lỗi liên quan đến cập nhật cơ sở dữ liệu
                Console.Error.WriteLine($"Database update error: {dbEx.Message}");
                return false;
            }
            catch (Exception ex)
            {
                // Xử lý các lỗi khác
                Console.Error.WriteLine($"An error occurred: {ex.Message}");
                return false;
            }
        }

        //Pham Son Tung
        public async Task<bool> RemoveOrder(int orderId)
        {
            try
            {
                // Tìm kiếm đơn hàng theo orderId
                var order = await _context.Orders
                    .Where(o => o.OrderId == orderId)
                    .FirstOrDefaultAsync();

                // Nếu không tìm thấy đơn hàng
                if (order == null)
                {
                    throw new Exception("Order not found");
                }

                // Chuyển OrderStatusId thành 3
                order.OrderStatusId = 3;

                // Cập nhật lại đơn hàng trong cơ sở dữ liệu
                _context.Orders.Update(order);

                // Lưu thay đổi vào cơ sở dữ liệu
                await _context.SaveChangesAsync();
                return true; // Thành công
            }
            catch (Exception ex)
            {
                // Ném lỗi ra ngoài nếu có lỗi xảy ra
                throw new Exception($"Error removing order: {ex.Message}", ex);
            }
        }

        //Pham Son Tung
        public async Task<IEnumerable<OrderDetail>> GetOrderDetailsByOrderIdAsync(int orderId)
        {
            try
            {
                var orderDetails = await _context.OrderDetails
                    .AsNoTracking()
                    .Where(od => od.OrderId == orderId)
                    .ToListAsync();

                return orderDetails;
            }
            catch (ArgumentNullException argEx)
            {
                throw new ArgumentException("Order ID must not be null.", argEx);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching order details by orderId.", ex);
            }
        }


        public async Task<List<Order>> GetAllAsync()
        {
            return await _context.Orders.ToListAsync();
        }

        public async Task AddOrderAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
        }

        public async Task AddOrderDetailsAsync(List<OrderDetail> orderDetails)
        {
            await _context.OrderDetails.AddRangeAsync(orderDetails);
            await _context.SaveChangesAsync();
        }


        //Pham Son Tung
        public async Task<Order> GetAllOrderData(int orderId)
        {
            try
            {
                var order = await _context.Orders
                    .Include(o => o.OrderDetails)
                    .FirstOrDefaultAsync(o => o.OrderId == orderId);

                if (order == null)
                {
                    throw new KeyNotFoundException($"Order with ID {orderId} was not found.");
                }
                return order;
            }
            catch (DbUpdateException dbEx)
            {
                throw new Exception("A database update error occurred while retrieving the order.", dbEx);
            }
            catch (InvalidOperationException invalidOpEx)
            {
                throw new Exception("An invalid operation occurred while retrieving the order.", invalidOpEx);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}
