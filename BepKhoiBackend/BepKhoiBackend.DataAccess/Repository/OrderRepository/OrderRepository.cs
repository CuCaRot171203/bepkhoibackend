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
        //-------------NgocQuan---------------//
        public async Task<Order?> GetOrderWithDetailsAsync(int orderId)
        {
            return await _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);
        }
    }
}
