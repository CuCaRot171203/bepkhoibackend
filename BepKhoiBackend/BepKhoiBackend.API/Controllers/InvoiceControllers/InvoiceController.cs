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

        //Phạm Sơn Tùng
        public class InvoicePaymentRequestDto
        {
            public InvoiceForPaymentDto InvoiceInfo { get; set; } = null!;
            public List<InvoiceDetailForPaymentDto> InvoiceDetails { get; set; } = new();
        }
        [HttpPost("create-invoice-for-payment")]
        public async Task<IActionResult> CreateInvoiceForPaymentAsync([FromBody] InvoicePaymentRequestDto request)
        {
            try
            {
                if (request == null || request.InvoiceInfo == null || request.InvoiceDetails == null || !request.InvoiceDetails.Any())
                {
                    return BadRequest(new { message = "Dữ liệu hóa đơn không hợp lệ." });
                }

                var invoice = request.InvoiceInfo;

                // Kiểm tra các điều kiện hợp lệ
                if (invoice.PaymentMethodId != 1 && invoice.PaymentMethodId != 2)
                    return BadRequest(new { message = "Phương thức thanh toán không hợp lệ. Chỉ chấp nhận 1 hoặc 2." });

                if (invoice.OrderTypeId < 1 || invoice.OrderTypeId > 3)
                    return BadRequest(new { message = "Loại đơn hàng không hợp lệ. Chỉ chấp nhận 1, 2 hoặc 3." });

                if (invoice.OrderId <= 0)
                    return BadRequest(new { message = "Mã đơn hàng không hợp lệ." });

                if (invoice.CashierId <= 0)
                    return BadRequest(new { message = "Mã thu ngân không hợp lệ." });

                if (invoice.CustomerId.HasValue && invoice.CustomerId <= 0)
                    return BadRequest(new { message = "Mã khách hàng không hợp lệ." });

                if (invoice.RoomId.HasValue && invoice.RoomId <= 0)
                    return BadRequest(new { message = "Mã phòng không hợp lệ." });

                if (invoice.ShipperId.HasValue && invoice.ShipperId <= 0)
                    return BadRequest(new { message = "Mã shipper không hợp lệ." });

                if (invoice.CheckInTime == default || invoice.CheckOutTime == default)
                    return BadRequest(new { message = "Thời gian check-in hoặc check-out không hợp lệ." });

                if (invoice.TotalQuantity <= 0)
                    return BadRequest(new { message = "Tổng số lượng sản phẩm phải lớn hơn 0." });

                foreach (var detail in request.InvoiceDetails)
                {
                    if (detail.ProductId <= 0)
                        return BadRequest(new { message = "Mã sản phẩm không hợp lệ." });

                    if (string.IsNullOrWhiteSpace(detail.ProductName))
                        return BadRequest(new { message = "Tên sản phẩm không được để trống." });

                    if (detail.Quantity <= 0)
                        return BadRequest(new { message = $"Số lượng của sản phẩm '{detail.ProductName}' phải lớn hơn 0." });
                }

                // Gọi service để tạo hóa đơn, nhận lại ID
                int createdInvoiceId = await _invoiceService.CreateInvoiceForPaymentServiceAsync(
                    request.InvoiceInfo,
                    request.InvoiceDetails
                );

                return Ok(new
                {
                    message = "Tạo hóa đơn thanh toán thành công.",
                    invoiceId = createdInvoiceId
                });
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new { message = "Lỗi CSDL khi tạo hóa đơn.", error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi không xác định.", error = ex.Message });
            }
        }




    }
}