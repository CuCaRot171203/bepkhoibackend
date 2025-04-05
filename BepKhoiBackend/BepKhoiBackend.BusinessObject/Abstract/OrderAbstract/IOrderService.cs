using BepKhoiBackend.BusinessObject.dtos.CustomerDto;
using BepKhoiBackend.BusinessObject.dtos.MenuDto;
using BepKhoiBackend.BusinessObject.dtos.OrderDetailDto;
using BepKhoiBackend.BusinessObject.dtos.OrderDto;
using BepKhoiBackend.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BepKhoiBackend.BusinessObject.Abstract.OrderAbstract
{
    public interface IOrderService
    {

        Task<OrderDto> CreateNewOrderAsync(CreateOrderRequestDto request);
        Task<OrderDto> AddOrderNoteToOrderPosAsync(AddNoteRequest request);
        Task<OrderDetailDto> UpdateOrderDetailQuantiyPosAsync(UpdateOrderDetailQuantityRequest request);
        Task<bool> AddCustomerToOrderAsync(AddCustomerToOrderRequest request);
        Task<bool> AddProductToOrderAsync(AddProductToOrderRequest request);
        Task<bool> ChangeOrderTypeServiceAsync(MoveOrderPosRequestDto request);
        Task<bool> CombineOrderPosServiceAsync(CombineOrderPosRequestDto request);
        Task<IEnumerable<OrderDtoPos>> GetOrdersByTypePosAsync(int? roomId, int? shipperId, int? orderTypeId);
        Task<CustomerPosDto> GetCustomerIdByOrderIdAsync(int orderId);
        Task AssignCustomerToOrderAsync(int orderId, int customerId);
        Task<ResultWithList<OrderDto>> GetAllOrdersAsync();


    }
}
