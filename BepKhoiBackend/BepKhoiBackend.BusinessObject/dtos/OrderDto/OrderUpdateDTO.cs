using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BepKhoiBackend.BusinessObject.dtos.OrderDto
{
    public class OrderUpdateDTO
    {
        public int OrderId { get; set; }
        public int? CustomerId { get; set; }
        public int OrderTypeId { get; set; }
        public int? RoomId { get; set; }
        public int TotalQuantity { get; set; }
        public decimal AmountDue { get; set; }
        public int OrderStatusId { get; set; }
        public string? OrderNote { get; set; }
        public List<OrderDetailUpdateDTO> OrderDetails { get; set; } = new();
    }

}
