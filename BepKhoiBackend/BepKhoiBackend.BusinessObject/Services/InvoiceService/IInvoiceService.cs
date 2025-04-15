using BepKhoiBackend.BusinessObject.dtos.InvoiceDto;
using BepKhoiBackend.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BepKhoiBackend.BusinessObject.Services.InvoiceService
{
    public interface IInvoiceService
    {
        List<InvoiceDTO> GetAllInvoices();
        InvoiceDTO? GetInvoiceById(int id);
        List<InvoiceDTO> GetInvoiceByCustomer(string keyword);
        List<InvoiceDTO> GetInvoiceByCashier(string keyword);
        List<InvoiceDTO> GetInvoiceByProductName(string productName);
        List<InvoiceDTO> GetInvoiceByPeriod(DateTime from, DateTime to);
        List<InvoiceDTO> GetInvoiceByStatus(bool status);
        List<InvoiceDTO> GetInvoiceByOrderMethod(string method);

        //------------------NgocQuan----------------------//
        InvoicePdfDTO GetInvoiceForPdf(int id);
        Invoice? GetInvoiceByInvoiceId(int id);
        bool UpdateInvoiceStatus(int invoiceId, bool status);
        Task<bool> CreateInvoiceForPaymentServiceAsync(InvoiceForPaymentDto invoiceDto, List<InvoiceDetailForPaymentDto> detailDtos);

    }

}
