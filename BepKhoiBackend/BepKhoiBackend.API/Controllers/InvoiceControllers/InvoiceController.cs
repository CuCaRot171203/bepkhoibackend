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
                return Ok(url);
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

            if (response.Success && response.VnPayResponseCode == "00")
            {
                if (int.TryParse(response.InvoiceId, out int invoiceId))
                {
                    _invoiceService.UpdateInvoiceStatus(invoiceId, true);

                    // Redirect đến frontend (ví dụ: trang thanh toán thành công)
                    var redirectUrl = $"https://facebook.com/";

                    //var redirectUrl = $"https://yourfrontend.com/payment-success?invoiceId={invoiceId}&transactionId={response.TransactionId}";
                    return Redirect(redirectUrl);
                }
            else
                {
                    var failUrl = $"https://www.facebook.com/reel/639253061931440";

                    //var failUrl = $"https://yourfrontend.com/payment-failure?message=InvalidInvoiceId";
                    return Redirect(failUrl);
                }
            }

            // Redirect đến trang thất bại
           var redirectFail = $"https://www.facebook.com/reel/639253061931440";

            //var redirectFail = $"https://yourfrontend.com/payment-failure?code={response.VnPayResponseCode}";
            return Redirect(redirectFail);
        }
    }
}