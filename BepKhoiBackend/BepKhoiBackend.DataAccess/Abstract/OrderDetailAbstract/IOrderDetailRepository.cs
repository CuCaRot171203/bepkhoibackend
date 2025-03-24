using BepKhoiBackend.DataAccess.Models;

namespace BepKhoiBackend.DataAccess.Abstract.OrderDetailAbstract
{
    public interface IOrderDetailRepository
    {
        Task<OrderDetail?> GetOrderDetailByIdAsync(int orderDetailId);
        Task CreateOrderCancellationHistoryAsync(OrderCancellationHistory orderCancellationHistory);
        Task DeleteOrderDetailAsync(OrderDetail orderDetail);
        Task AddOrderDetailAsync(OrderDetail orderDetail);
        Task UpdateOrderDetailAsync(OrderDetail orderDetail);
        Task<OrderDetail?> GetOrderDetailByProductAsync(int orderId, int productId);
    }
}
