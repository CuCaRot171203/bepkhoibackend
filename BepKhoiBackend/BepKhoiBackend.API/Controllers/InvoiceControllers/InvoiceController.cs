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
        private readonly VnPayService _vnPayService;
        private readonly PrintInvoicePdfService _pdfService;
        public InvoiceController(IInvoiceService invoiceService, VnPayService vnPayService, PrintInvoicePdfService pdfService)
        {
            _invoiceService = invoiceService;
            _vnPayService = vnPayService;
            _pdfService = pdfService;
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

        //------------------NgocQuan----------------------//
        [HttpGet("{id}/print-pdf")]
        public IActionResult GetInvoicePdf(int id)
        {
            var invoice = _invoiceService.GetInvoiceForPdf(id);

            if (invoice == null)
            {
                return NotFound($"Không tìm thấy hóa đơn với ID {id}");
            }

            var pdfBytes = _pdfService.GenerateInvoicePdf(invoice);
            return File(pdfBytes, "application/pdf", $"Invoice_{id}.pdf");
        }

        [HttpGet("vnpay-url")]
        public IActionResult CreatePaymentUrlVnpay([FromQuery] int Id)
        {
            var invoice = _invoiceService.GetInvoiceByInvoiceId(Id);
            if (invoice == null)
            {
                return NotFound($"Không tìm thấy hóa đơn với ID {Id}");
            }

            if (invoice.AmountDue == null || invoice.AmountDue <= 0)
            {
                return BadRequest("Số tiền thanh toán không hợp lệ.");
            }

            var model = new PaymentInformationModel
            {
                OrderType = "other",
                Amount = (int)invoice.AmountDue,
                InvoiceId = invoice.InvoiceId.ToString(),
                Name = invoice.Customer?.CustomerName ?? "Khách Lẻ"
            };

            try
            {
                var url = _vnPayService.CreatePaymentUrl(model, HttpContext);
                return Redirect(url);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Có lỗi xảy ra trong quá trình tạo URL thanh toán.");
            }
        }


        [HttpGet("Return")]
        public IActionResult PaymentCallbackVnpay()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);

            // Kiểm tra thanh toán thành công với mã "00"
            if (response.Success && response.VnPayResponseCode == "00")
            {
                if (int.TryParse(response.InvoiceId, out int invoiceId))
                {
                    _invoiceService.UpdateInvoiceStatus(invoiceId, true);
                    return Ok(new
                    {
                        success = true,
                        message = "Thanh toán thành công!",
                        invoiceId = invoiceId,
                        transactionId = response.TransactionId
                    });
                }
                else
                {
                    return BadRequest("Không thể xác định ID hóa đơn.");
                }
            }

            return BadRequest(new
            {
                success = false,
                message = "Thanh toán thất bại hoặc bị hủy.",
                code = response.VnPayResponseCode
            });
        }

    }
}
