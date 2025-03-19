using BepKhoiBackend.BusinessObject.dtos.CustomerDto;
using BepKhoiBackend.BusinessObject.Services.CustomerService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BepKhoiBackend.API.Controllers.CustomerControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        /// <summary>
        /// Lấy danh sách tất cả khách hàng
        /// </summary>
        [HttpGet]
        public ActionResult<List<CustomerDTO>> GetAllCustomers()
        {
            var customers = _customerService.GetAllCustomers();
            return Ok(customers);
        }

        /// <summary>
        /// Tìm khách hàng theo ID
        /// </summary>
        [HttpGet("{id}")]
        public ActionResult<CustomerDTO> GetCustomerById(int id)
        {
            var customer = _customerService.GetCustomerById(id);
            if (customer == null)
            {
                return NotFound(new { message = "Khách hàng không tồn tại!" });
            }
            return Ok(customer);
        }

        /// <summary>
        /// Tìm kiếm khách hàng theo tên
        /// </summary>
        [HttpGet("search")]
        public ActionResult<List<CustomerDTO>> SearchCustomersByNameOrPhone([FromQuery] string searchTerm)
        {
            var customers = _customerService.SearchCustomers(searchTerm);
            return Ok(customers);
        }
        [HttpGet("{customerId}/invoices")]
        public IActionResult GetInvoicesByCustomerId(int customerId)
        {
            var invoices = _customerService.GetInvoicesByCustomerId(customerId);
            if (invoices == null || invoices.Count == 0)
            {
                return NotFound("Không tìm thấy hóa đơn nào cho khách hàng này.");
            }
            return Ok(invoices);
        }
        [HttpGet("export")]
        public IActionResult ExportCustomers()
        {
            var fileContents = _customerService.ExportCustomersToExcel();
            return File(fileContents, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Customers.xlsx");
        }
    
    }
}
