using BepKhoiBackend.DataAccess.Models;

namespace BepKhoiBackend.DataAccess.Abstract.OrderAbstract
{
    public interface IOrderRepository
    {
        Task<Order> CreateOrderPosAsync(Order order);
        Task<Order?> GetOrderByIdPosAsync(int orderId);
        Task<Order> UpdateOrderAsyncPos(Order order);
        Task<OrderDetail> GetOrderDetailByIdAsync(int orderDetailId);
        Task UpdateOrderDetailAsync(OrderDetail orderDetail);
        Task<Customer?> GetCustomerByIdAsync(int customerId);
        Task UpdateOrderAsync(Order order);
        Task<Menu?> GetProductByIdAsync(int productId);
        Task<bool> MoveOrderPosRepoAsync(int orderId, int orderTypeId, int? roomId, int? userId);
        Task<bool> CombineOrderPosRepoAsync(int firstOrderId, int secondOrderId);
        Task<IEnumerable<Order>> GetOrdersByTypePos(int? roomId, int? shipperId, int? orderTypeId);
        //-------------NgocQuan---------------//
        Task<Order?> GetOrderWithDetailsAsync(int orderId);
        Task<Customer> GetCustomerIdByOrderIdAsync(int orderId);
        Task AssignCustomerToOrder(int orderId, int customerId);
        Task<bool> RemoveCustomerFromOrderAsync(int orderId);
        Task<bool> RemoveOrder(int orderId);
        Task<IEnumerable<OrderDetail>> GetOrderDetailsByOrderIdAsync(int orderId);
        Task<List<Order>> GetAllAsync();
        Task AddOrderAsync(Order order);
        Task AddOrderDetailsAsync(List<OrderDetail> orderDetails);


    }
}
