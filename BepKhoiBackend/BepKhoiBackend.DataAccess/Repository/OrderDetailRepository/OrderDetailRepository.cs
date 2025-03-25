using BepKhoiBackend.DataAccess.Abstract.OrderDetailAbstract;
using BepKhoiBackend.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace BepKhoiBackend.DataAccess.Repository.OrderDetailRepository
{
    public class OrderDetailRepository : IOrderDetailRepository
    {
        public readonly bepkhoiContext _context;

        public OrderDetailRepository(bepkhoiContext context)
        {
            _context = context;
        }

        // Task for get order detail by Id
        public async Task<OrderDetail?> GetOrderDetailByIdAsync(int orderDetailId)
        {
            return await _context.OrderDetails
                .AsNoTracking()
                .FirstOrDefaultAsync(od => od.OrderDetailId == orderDetailId);
        }

        // Task for create cancellation

        public async Task CreateOrderCancellationHistoryAsync(OrderCancellationHistory orderCancellationHistory)
        {
            _context.OrderCancellationHistories.Add(orderCancellationHistory);
            await _context.SaveChangesAsync();
        }


        // Task for delete order detail
        public async Task DeleteOrderDetailAsync(OrderDetail orderDetail)
        {
            _context.OrderDetails.Remove(orderDetail);
            await _context.SaveChangesAsync();
        }

        // Func to add orderdetail
        public async Task AddOrderDetailAsync(OrderDetail orderDetail)
        {
            await _context.OrderDetails.AddAsync(orderDetail);
            await _context.SaveChangesAsync();
        }

        // Func to update order detail
        public async Task UpdateOrderDetailAsync(OrderDetail orderDetail)
        {
            _context.OrderDetails.Update(orderDetail);
            await _context.SaveChangesAsync();
        }

        // Func to get order detail by product id
        public async Task<OrderDetail?> GetOrderDetailByProductAsync(int orderId, int productId)
        {
            return await _context.OrderDetails
                .FirstOrDefaultAsync(od => od.OrderId == orderId && od.ProductId == productId);
        }
    }
}
