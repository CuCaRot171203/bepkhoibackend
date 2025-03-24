using AutoMapper;
using BepKhoiBackend.BusinessObject.Abstract.OrderDetailAbstract;
using BepKhoiBackend.BusinessObject.dtos.OrderDetailDto;
using BepKhoiBackend.DataAccess.Abstract.OrderAbstract;
using BepKhoiBackend.DataAccess.Abstract.OrderDetailAbstract;
using BepKhoiBackend.DataAccess.Models;
using BepKhoiBackend.DataAccess.Repository.OrderRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BepKhoiBackend.BusinessObject.Services.OrderDetailService
{
    public class OrderDetailService : IOrderDetailService
    {
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly IMapper _mapper;
        private readonly bepkhoiContext _context;

        public OrderDetailService(IOrderDetailRepository orderDetailRepository, IMapper mapper, bepkhoiContext context)
        {
            _orderDetailRepository = orderDetailRepository;
            _mapper = mapper;
            _context = context;
        }

        public async Task<bool> CancelOrderDetailAsync(CancelOrderDetailRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var orderDetail = await _orderDetailRepository.GetOrderDetailByIdAsync(request.OrderDetailId);

                if(orderDetail == null)
                {
                    throw new KeyNotFoundException("Order detail not found.");
                }

                if (orderDetail.Status != true)
                {
                    throw new ArgumentException("Order detail can only be canceled after being sent to the kitchen.");
                }

                if (orderDetail.Quantity < request.Quantity)
                {
                    throw new ArgumentException("Cancel quantity cannot exceed the current order quantity.");
                }

                // Update OrderDetail
                orderDetail.Quantity -= request.Quantity;

                if (orderDetail.Quantity == 0)
                {
                    _context.OrderDetails.Remove(orderDetail);
                }
                else
                {
                    _context.OrderDetails.Update(orderDetail);
                }

                await _context.SaveChangesAsync();

                // Create OrderCancellationHistory
                var cancellationHistory = new OrderCancellationHistory
                {
                    OrderId = request.OrderId,
                    CashierId = request.CashierId,
                    ProductId = orderDetail.ProductId,
                    Quantity = request.Quantity,
                    Reason = request.Reason
                };

                await _orderDetailRepository.CreateOrderCancellationHistoryAsync(cancellationHistory);

                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // Method to remove
        public async Task<bool> RemoveOrderDetailAsync(RemoveOrderDetailRequest request)
        {
            try
            {
                var orderDetail = await _orderDetailRepository.GetOrderDetailByIdAsync(request.OrderDetailId);

                //Check null
                if(orderDetail == null)
                {
                    //throw new KeyNotFoundException($"Order detail with ID {request.OrderDetailId} not found.");
                    return false;
                }

                if(orderDetail.Status == true)
                {
                    throw new ArgumentException("Order detail cannot be removed after being sent to the kitchen.");
                }

                _context.OrderDetails.Remove(orderDetail);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> AddNoteToOrderDetailAsync(AddNoteToOrderDetailRequest request)
        {
            try
            {
                var orderDetail = await _orderDetailRepository.GetOrderDetailByIdAsync(request.OrderDetailId);

                // Check null
                if (orderDetail == null)
                {
                    throw new KeyNotFoundException("Order detail not found.");
                }

                // check matched
                if(orderDetail.OrderId != request.OrderId)
                {
                    throw new ArgumentException("Order ID mismatch.");
                }

                orderDetail.ProductNote = request.Note;
                _context.OrderDetails.Update(orderDetail);
                await _context.SaveChangesAsync();

                return true;
            }
            catch
            {
                throw;
            }
        }
    }
}
