using BepKhoiBackend.DataAccess.Models;
using BepKhoiBackend.DataAccess.Repository.InvoiceRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BepKhoiBackend.DataAccess.Repositories
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly bepkhoiContext _context;

        public InvoiceRepository(bepkhoiContext context)
        {
            _context = context;
        }

        public List<Invoice> GetAllInvoices()
        {
            return _context.Invoices
                .Include(i => i.InvoiceDetails)
                .ToList();
        }

        public Invoice? GetInvoiceById(int id)
        {
            return _context.Invoices
                .Include(i => i.InvoiceDetails)
                .FirstOrDefault(i => i.InvoiceId == id);
        }

        public List<Invoice> GetInvoiceByCustomer(string keyword)
        {
            return _context.Invoices
                .Include(i => i.Customer)
                .Include(i => i.InvoiceDetails)
                .Where(i => i.CustomerId.ToString() == keyword ||
                            i.Customer!.CustomerName.Contains(keyword) ||
                            i.Customer.Phone.Contains(keyword))
                .ToList();
        }

        public List<Invoice> GetInvoiceByCashier(string keyword)
        {
            return _context.Invoices
                .Include(i => i.Cashier)
                .Include(i => i.InvoiceDetails)
                .Where(i => i.CashierId.ToString() == keyword ||
                            i.Cashier.UserInformation.UserName.Contains(keyword))
                .ToList();
        }

        public List<Invoice> GetInvoiceByProductName(string productName)
        {
            return _context.Invoices
                .Include(i => i.InvoiceDetails)
                .ThenInclude(d => d.Product)
                .Where(i => i.InvoiceDetails.Any(d => d.Product.ProductName.Contains(productName)))
                .ToList();
        }

        public List<Invoice> GetInvoiceByPeriod(DateTime from, DateTime to)
        {
            return _context.Invoices
                 .Include(i => i.InvoiceDetails)
                .Where(i => i.CheckInTime >= from && i.CheckOutTime <= to)
                .ToList();
        }

        public List<Invoice> GetInvoiceByStatus(bool status)
        {
            return _context.Invoices
                .Include(i => i.InvoiceDetails)
                .Where(i => i.Status == status)
                .ToList();
        }

        public List<Invoice> GetInvoiceByOrderMethod(string method)
        {
            return _context.Invoices
              .Include(i => i.InvoiceDetails)
                .Include(i => i.PaymentMethod)
                .Where(i => i.PaymentMethod.PaymentMethodTitle.Contains(method))
                .ToList();
        }
    }
}
