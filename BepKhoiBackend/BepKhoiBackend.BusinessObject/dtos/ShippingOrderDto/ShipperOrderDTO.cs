﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BepKhoiBackend.BusinessObject.dtos.ShippingOrderDto
{
    public class ShipperOrderDTO
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public List<OrderDTO> OrderList { get; set; } = new();
    }
}
