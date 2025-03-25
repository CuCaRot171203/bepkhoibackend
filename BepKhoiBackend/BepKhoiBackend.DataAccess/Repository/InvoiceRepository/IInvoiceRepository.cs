using BepKhoiBackend.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BepKhoiBackend.DataAccess.Repository.InvoiceRepository
{
    public interface IInvoiceRepository
    {
        List<Invoice> GetAllInvoices();
        Invoice? GetInvoiceById(int id);
        List<Invoice> GetInvoiceByCustomer(string keyword);
        List<Invoice> GetInvoiceByCashier(string keyword);
        List<Invoice> GetInvoiceByProductName(string productName);
        List<Invoice> GetInvoiceByPeriod(DateTime from, DateTime to);
        List<Invoice> GetInvoiceByStatus(bool status);
        List<Invoice> GetInvoiceByOrderMethod(string method);
    }
}
