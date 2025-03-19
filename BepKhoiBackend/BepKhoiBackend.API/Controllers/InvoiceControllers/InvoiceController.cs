using BepKhoiBackend.BusinessObject.dtos.InvoiceDto;
using BepKhoiBackend.BusinessObject.Services.InvoiceService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BepKhoiBackend.API.Controllers.InvoiceControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoiceService _invoiceService;

        public InvoiceController(IInvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
        }

        [HttpGet]
        public ActionResult<List<InvoiceDTO>> GetAllInvoices()
        {
            return Ok(_invoiceService.GetAllInvoices());
        }

        [HttpGet("{id}")]
        public ActionResult<InvoiceDTO> GetInvoiceById(int id)
        {
            var invoice = _invoiceService.GetInvoiceById(id);
            if (invoice == null) return NotFound();
            return Ok(invoice);
        }

        [HttpGet("customer/{keyword}")]
        public ActionResult<List<InvoiceDTO>> GetInvoiceByCustomer(string keyword)
        {
            return Ok(_invoiceService.GetInvoiceByCustomer(keyword));
        }

        [HttpGet("cashier/{keyword}")]
        public ActionResult<List<InvoiceDTO>> GetInvoiceByCashier(string keyword)
        {
            return Ok(_invoiceService.GetInvoiceByCashier(keyword));
        }

        [HttpGet("product/{productName}")]
        public ActionResult<List<InvoiceDTO>> GetInvoiceByProductName(string productName)
        {
            return Ok(_invoiceService.GetInvoiceByProductName(productName));
        }

        [HttpGet("period")]
        public ActionResult<List<InvoiceDTO>> GetInvoiceByPeriod([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            return Ok(_invoiceService.GetInvoiceByPeriod(from, to));
        }

        [HttpGet("status/{status}")]
        public ActionResult<List<InvoiceDTO>> GetInvoiceByStatus(bool status)
        {
            return Ok(_invoiceService.GetInvoiceByStatus(status));
        }

        [HttpGet("order-method/{method}")]
        public ActionResult<List<InvoiceDTO>> GetInvoiceByOrderMethod(string method)
        {
            return Ok(_invoiceService.GetInvoiceByOrderMethod(method));
        }
    }
}
