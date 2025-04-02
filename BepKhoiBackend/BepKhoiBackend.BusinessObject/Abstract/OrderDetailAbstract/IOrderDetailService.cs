using BepKhoiBackend.BusinessObject.dtos.OrderDetailDto;

namespace BepKhoiBackend.BusinessObject.Abstract.OrderDetailAbstract
{
    public interface IOrderDetailService
    {
        Task<bool> CancelOrderDetailAsync(CancelOrderDetailRequest request);
        Task<bool> RemoveOrderDetailAsync(RemoveOrderDetailRequest request);
        Task<bool> AddNoteToOrderDetailAsync(AddNoteToOrderDetailRequest request);
        Task<bool> ConfirmOrderPosServiceAsync(int orderId);
        Task<bool> SplitOrderPosServiceAsync(SplitOrderPosRquest request);
    }
}
