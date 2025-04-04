using BepKhoiBackend.BusinessObject.dtos.InvoiceDto;
using BepKhoiBackend.DataAccess.Models;
using BepKhoiBackend.DataAccess.Repository.InvoiceRepository;
using DocumentFormat.OpenXml.Office2010.Excel;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
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

    }
}
