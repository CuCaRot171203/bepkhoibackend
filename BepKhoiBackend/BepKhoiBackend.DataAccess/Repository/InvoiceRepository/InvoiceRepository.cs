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
                .Include(i => i.PaymentMethod)
                .Include(i => i.OrderType)
                .Include(i => i.Cashier).ThenInclude(i => i.UserInformation)
                .Include(i => i.Shipper).ThenInclude(i => i.UserInformation)
                .Include(i => i.Customer)
                .Include(i => i.Room)
                .ToList();
        }


        public Invoice? GetInvoiceById(int id)
        {
            return _context.Invoices
                .Include(i => i.InvoiceDetails)
                .Include(i => i.PaymentMethod)
                .Include(i => i.OrderType)
                .Include(i => i.Cashier).ThenInclude(i => i.UserInformation)
                .Include(i => i.Shipper).ThenInclude(i => i.UserInformation)
                .Include(i => i.Customer)
                .Include(i => i.Room)
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

        //------------------NgocQuan----------------------//
        public Invoice? GetInvoiceForPdf(int id)
        {
            return _context.Invoices
                .Include(i => i.InvoiceDetails)
                .ThenInclude(d => d.Product)
                .Include(i => i.Customer)
                .FirstOrDefault(i => i.InvoiceId == id);
        }
        public async Task<Invoice> GetInvoiceByIdAsync(int id)
        {
            return await _context.Invoices.FirstOrDefaultAsync(i => i.InvoiceId == id);
        }
        public async Task<bool> UpdateInvoiceStatus(int invoiceId, bool status)
        {
            var invoice = await _context.Invoices.FirstOrDefaultAsync(i => i.InvoiceId == invoiceId);

            // Nếu hóa đơn không tồn tại, trả về false
            if (invoice == null)
            {
                return false;
            }

            // Cập nhật trạng thái của hóa đơn
            invoice.Status = status;

            // Lưu thay đổi vào cơ sở dữ liệu
            _context.SaveChanges();

            return true;
        }

        //Phạm Sơn Tùng
        public async Task<Invoice> CreateInvoiceForPaymentAsync(Invoice invoice)
        {
            if (invoice == null)
            {
                throw new ArgumentNullException(nameof(invoice), "Invoice object cannot be null.");
            }

            try
            {
                await _context.Invoices.AddAsync(invoice);
                await _context.SaveChangesAsync();
                return invoice;
            }
            catch (DbUpdateException dbEx)
            {
                throw new DbUpdateException("Database update error while creating invoice.", dbEx);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while creating invoice.", ex);
            }
        }

        //phạm sơn tùng
        public async Task<bool> AddInvoiceDetailForPaymentsAsync(List<InvoiceDetail> invoiceDetails)
        {
            try
            {
                if (invoiceDetails == null || !invoiceDetails.Any())
                    throw new ArgumentException("Empty invoice detail list.");
                await _context.InvoiceDetails.AddRangeAsync(invoiceDetails);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (DbUpdateException dbEx)
            {
                throw new DbUpdateException("Database error occur when saving invoice detail.", dbEx);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error has been occur when saving invoice detail.", ex);
            }
        }
        //Phạm Sơn Tùng
        public async Task ChangeOrderStatusAfterPayment(int orderId)
        {
            try
            {
                var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);

                if (order == null)
                {
                    throw new InvalidOperationException($"Order with ID {orderId} not found.");
                }
                order.OrderStatusId = 2;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException dbEx)
            {
                throw new Exception("An error occurred while updating the order status in the database.", dbEx);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the order status.", ex);
            }
        }
    }
}
