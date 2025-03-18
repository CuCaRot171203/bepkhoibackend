using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepKhoiBackend.BusinessObject.dtos.CustomerDto;

namespace BepKhoiBackend.BusinessObject.Services.CustomerService
{
    public interface ICustomerService
    {
        List<CustomerDTO> GetAllCustomers();
        CustomerDTO? GetCustomerById(int customerId);
        List<CustomerDTO> SearchCustomers(string searchTerm);
        List<CustomerInvoiceDto> GetInvoicesByCustomerId(int customerId);
        byte[] ExportCustomersToExcel();
       


    }
}
