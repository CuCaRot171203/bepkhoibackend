﻿using BepKhoiBackend.BusinessObject.dtos.CustomerDto;
using BepKhoiBackend.BusinessObject.dtos.MenuDto;
using BepKhoiBackend.BusinessObject.dtos.OrderDetailDto;
using BepKhoiBackend.BusinessObject.dtos.OrderDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BepKhoiBackend.BusinessObject.Abstract.OrderAbstract
{
    public interface IOrderService
    {
        Task<OrderDto> CreateNewOrderAsync(CreateOrderRequest request);
        Task<OrderDto> AddOrderNoteToOrderPosAsync(AddNoteRequest request);
        Task<OrderDetailDto> UpdateOrderDetailQuantiyPosAsync(UpdateOrderDetailQuantityRequest request);
        Task<bool> AddCustomerToOrderAsync(AddCustomerToOrderRequest request);
        Task<bool> AddProductToOrderAsync(AddProductToOrderRequest request);
    }
}
