using BepKhoiBackend.BusinessObject.dtos.InvoiceDto;
using BepKhoiBackend.DataAccess.Models;
using BepKhoiBackend.DataAccess.Repository.InvoiceRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BepKhoiBackend.BusinessObject.Services.InvoiceService
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepository _invoiceRepository;

        public InvoiceService(IInvoiceRepository invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
        }

        public List<InvoiceDTO> GetAllInvoices()
        {
            return _invoiceRepository.GetAllInvoices()
                .Select(i => new InvoiceDTO
                {
                    InvoiceId = i.InvoiceId,
                    InvoiceDetails = i.InvoiceDetails.Select(d => new InvoiceDetailDTO
                    {
                        InvoiceDetailId = d.InvoiceDetailId,
                        ProductId = d.ProductId,
                        ProductName = d.ProductName,
                        Quantity = d.Quantity,
                        Price = d.Price,
                        ProductVat = d.ProductVat,
                        ProductNote = d.ProductNote
                    }).ToList()
                }).ToList();
        }

        public InvoiceDTO? GetInvoiceById(int id)
        {
            var invoice = _invoiceRepository.GetInvoiceById(id);
            if (invoice == null) return null;
            return new InvoiceDTO
            {
                InvoiceId = invoice.InvoiceId,
                InvoiceDetails = invoice.InvoiceDetails.Select(d => new InvoiceDetailDTO
                {
                    InvoiceDetailId = d.InvoiceDetailId,
                    ProductId = d.ProductId,
                    ProductName = d.ProductName,
                    Quantity = d.Quantity,
                    Price = d.Price,
                    ProductVat = d.ProductVat,
                    ProductNote = d.ProductNote
                }).ToList()
            };
        }

        public List<InvoiceDTO> GetInvoiceByCustomer(string keyword)
        {
            return _invoiceRepository.GetInvoiceByCustomer(keyword)
                .Select(i => new InvoiceDTO
                {
                    InvoiceId = i.InvoiceId,
                    InvoiceDetails = i.InvoiceDetails.Select(d => new InvoiceDetailDTO
                    {
                        InvoiceDetailId = d.InvoiceDetailId,
                        ProductId = d.ProductId,
                        ProductName = d.ProductName,
                        Quantity = d.Quantity,
                        Price = d.Price,
                        ProductVat = d.ProductVat,
                        ProductNote = d.ProductNote
                    }).ToList()
                }).ToList();
        }

        public List<InvoiceDTO> GetInvoiceByCashier(string keyword)
        {
            return _invoiceRepository.GetInvoiceByCashier(keyword)
                .Select(i => new InvoiceDTO
                {
                    InvoiceId = i.InvoiceId,
                    InvoiceDetails = i.InvoiceDetails.Select(d => new InvoiceDetailDTO
                    {
                        InvoiceDetailId = d.InvoiceDetailId,
                        ProductId = d.ProductId,
                        ProductName = d.ProductName,
                        Quantity = d.Quantity,
                        Price = d.Price,
                        ProductVat = d.ProductVat,
                        ProductNote = d.ProductNote
                    }).ToList()
                }).ToList();
        }

        public List<InvoiceDTO> GetInvoiceByProductName(string productName)
        {
            return _invoiceRepository.GetInvoiceByProductName(productName)
                .Select(i => new InvoiceDTO
                {
                    InvoiceId = i.InvoiceId,
                    InvoiceDetails = i.InvoiceDetails.Select(d => new InvoiceDetailDTO
                    {
                        InvoiceDetailId = d.InvoiceDetailId,
                        ProductId = d.ProductId,
                        ProductName = d.ProductName,
                        Quantity = d.Quantity,
                        Price = d.Price,
                        ProductVat = d.ProductVat,
                        ProductNote = d.ProductNote
                    }).ToList()
                }).ToList();
        }

        public List<InvoiceDTO> GetInvoiceByPeriod(DateTime from, DateTime to)
        {
            return _invoiceRepository.GetInvoiceByPeriod(from, to)
                .Select(i => new InvoiceDTO
                {
                    InvoiceId = i.InvoiceId,
                    InvoiceDetails = i.InvoiceDetails.Select(d => new InvoiceDetailDTO
                    {
                        InvoiceDetailId = d.InvoiceDetailId,
                        ProductId = d.ProductId,
                        ProductName = d.ProductName,
                        Quantity = d.Quantity,
                        Price = d.Price,
                        ProductVat = d.ProductVat,
                        ProductNote = d.ProductNote
                    }).ToList()
                }).ToList();
        }

        public List<InvoiceDTO> GetInvoiceByStatus(bool status)
        {
            return _invoiceRepository.GetInvoiceByStatus(status)
                .Select(i => new InvoiceDTO
                {
                    InvoiceId = i.InvoiceId,
                    InvoiceDetails = i.InvoiceDetails.Select(d => new InvoiceDetailDTO
                    {
                        InvoiceDetailId = d.InvoiceDetailId,
                        ProductId = d.ProductId,
                        ProductName = d.ProductName,
                        Quantity = d.Quantity,
                        Price = d.Price,
                        ProductVat = d.ProductVat,
                        ProductNote = d.ProductNote
                    }).ToList()
                }).ToList();
        }

        public List<InvoiceDTO> GetInvoiceByOrderMethod(string method)
        {
            return _invoiceRepository.GetInvoiceByOrderMethod(method)
                .Select(i => new InvoiceDTO
                {
                    InvoiceId = i.InvoiceId,
                    InvoiceDetails = i.InvoiceDetails.Select(d => new InvoiceDetailDTO
                    {
                        InvoiceDetailId = d.InvoiceDetailId,
                        ProductId = d.ProductId,
                        ProductName = d.ProductName,
                        Quantity = d.Quantity,
                        Price = d.Price,
                        ProductVat = d.ProductVat,
                        ProductNote = d.ProductNote
                    }).ToList()
                }).ToList();
        }

        //------------------NgocQuan----------------------//
        public InvoicePdfDTO GetInvoiceForPdf(int id)
        {
            var invoice = _invoiceRepository.GetInvoiceForPdf(id);
            if (invoice == null) return null;

            return new InvoicePdfDTO
            {
                InvoiceId = invoice.InvoiceId,
                CheckInTime = invoice.CheckInTime,
                CheckOutTime = invoice.CheckOutTime,
                CustomerName = invoice.Customer?.CustomerName ?? "Khách lẻ",
                TotalQuantity = invoice.TotalQuantity,
                Subtotal = invoice.Subtotal,
                TotalVat = invoice.TotalVat ?? 0,
                AmountDue = invoice.AmountDue,
                InvoiceDetails = invoice.InvoiceDetails.Select(d => new InvoiceDetailPdfDTO
                {
                    ProductName = d.Product != null ? d.Product.ProductName : "Không xác định",
                    Quantity = d.Quantity,
                    Price = d.Price
                }).ToList()
            };
        }


        public Invoice? GetInvoiceByInvoiceId(int invoiceId)
        {
            var invoice = _invoiceRepository.GetInvoiceById(invoiceId);
            return invoice;
        }

        public bool UpdateInvoiceStatus(int invoiceId, bool status)
        {
            // Gọi phương thức cập nhật trạng thái hóa đơn trong repository
            bool isUpdated = _invoiceRepository.UpdateInvoiceStatus(invoiceId, status);

            // Kiểm tra nếu không thành công (ví dụ: không tìm thấy hóa đơn)
            if (!isUpdated)
            {
                // Thực hiện xử lý khi không thể cập nhật trạng thái hóa đơn
                return false;  // Trả về false nếu không thành công
            }

            // Trả về true nếu việc cập nhật thành công
            return true;
        }

        //Phạm sơn tùng
        public async Task<bool> CreateInvoiceForPaymentServiceAsync(InvoiceForPaymentDto invoiceDto,List<InvoiceDetailForPaymentDto> detailDtos)
        {
            if (invoiceDto == null) throw new ArgumentNullException(nameof(invoiceDto));
            if (detailDtos == null || !detailDtos.Any()) throw new ArgumentException("Chi tiết hóa đơn không được để trống.");

            try
            {
                // 1. Tạo thực thể Invoice từ DTO
                var invoice = new Invoice
                {
                    PaymentMethodId = invoiceDto.PaymentMethodId,
                    OrderId = invoiceDto.OrderId,
                    OrderTypeId = invoiceDto.OrderTypeId,
                    CashierId = invoiceDto.CashierId,
                    ShipperId = invoiceDto.ShipperId,
                    CustomerId = invoiceDto.CustomerId,
                    RoomId = invoiceDto.RoomId,
                    CheckInTime = invoiceDto.CheckInTime,
                    CheckOutTime = invoiceDto.CheckOutTime,
                    TotalQuantity = invoiceDto.TotalQuantity,
                    Subtotal = invoiceDto.Subtotal,
                    OtherPayment = invoiceDto.OtherPayment,
                    InvoiceDiscount = invoiceDto.InvoiceDiscount,
                    TotalVat = invoiceDto.TotalVat,
                    AmountDue = invoiceDto.AmountDue,
                    Status = invoiceDto.Status ?? true,
                    InvoiceNote = invoiceDto.InvoiceNote
                };

                // 2. Gọi repo để lưu Invoice
                var createdInvoice = await _invoiceRepository.CreateInvoiceForPaymentAsync(invoice);

                // 3. Chuyển đổi detailDtos sang danh sách InvoiceDetail với invoiceId vừa tạo
                var invoiceDetails = detailDtos.Select(d => new InvoiceDetail
                {
                    InvoiceId = createdInvoice.InvoiceId,
                    ProductId = d.ProductId,
                    ProductName = d.ProductName,
                    Quantity = d.Quantity,
                    Price = d.Price,
                    ProductVat = d.ProductVat,
                    ProductNote = d.ProductNote
                }).ToList();

                // 4. Gọi repo để lưu các InvoiceDetail
                var isDetailSaved = await _invoiceRepository.AddInvoiceDetailForPaymentsAsync(invoiceDetails);
                await _invoiceRepository.ChangeOrderStatusAfterPayment(invoiceDto.OrderId);
                return isDetailSaved;
            }
            catch (DbUpdateException dbEx)
            {
                throw new DbUpdateException("Database update error in service in CreateInvoiceForPaymentAsync.", dbEx);
            }
            catch (Exception ex)
            {
                throw new Exception("Undefined Error in service in CreateInvoiceForPaymentAsync..", ex);
            }
        }  




    }
}
