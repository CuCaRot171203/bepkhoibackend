using BepKhoiBackend.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BepKhoiBackend.DataAccess.Repository.CustomerRepository
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly bepkhoiContext _context;

        public CustomerRepository(bepkhoiContext context)
        {
            _context = context;
        }

        public List<Customer> GetAllCustomers()
        {
            return _context.Customers
                .Include(c => c.Invoices) // Load danh sách hóa đơn
                .Where(c => c.IsDelete == false || c.IsDelete == null)
                .ToList();
        }

        public Customer? GetCustomerById(int customerId)
        {
            return _context.Customers
                .Include(c => c.Invoices) // Load danh sách hóa đơn
                .FirstOrDefault(c => c.CustomerId == customerId && (c.IsDelete == false || c.IsDelete == null));
        }

        public List<Customer> SearchCustomers(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword)) return new List<Customer>();
            keyword = keyword.Trim();

            return _context.Customers
                .Include(c => c.Invoices) // Load danh sách hóa đơn
                .Where(c => (c.CustomerName.Contains(keyword) || c.Phone.Contains(keyword))
                            && (c.IsDelete == false || c.IsDelete == null))
                .ToList();
        }

    }
}
