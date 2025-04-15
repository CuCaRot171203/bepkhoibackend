using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BepKhoiBackend.BusinessObject.dtos.InvoiceDto.VnPay
{
   
        public class RequestDto
        {
            public string orderId { get; set; }
            public decimal amountDue { get; set; }
            public string customerName { get; set; }

        }
}
