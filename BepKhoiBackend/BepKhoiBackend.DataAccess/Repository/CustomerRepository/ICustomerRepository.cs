using BepKhoiBackend.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BepKhoiBackend.DataAccess.Repository.CustomerRepository
{
    public interface ICustomerRepository
    {
        List<Customer> GetAllCustomers();
        Customer? GetCustomerById(int customerId);
        List<Customer> SearchCustomers(string keyword); // Gộp tìm kiếm theo tên và số điện thoại
    }
}
