using BepKhoiBackend.BusinessObject.dtos.OrderDetailDto;

namespace BepKhoiBackend.BusinessObject.dtos.OrderDto
{
    public class OrderDtoPos
    {
        public int OrderId { get; set; }
        public int? CustomerId { get; set; }
        public DateTime CreatedTime { get; set; }
        public decimal AmountDue { get; set; }
        public List<OrderDetailDtoPos> OrderDetails { get; set; } = new();
    }
}
