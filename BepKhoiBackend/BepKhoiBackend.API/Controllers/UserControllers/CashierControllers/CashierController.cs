using BepKhoiBackend.BusinessObject.dtos.UserDto.CashierDto;
using BepKhoiBackend.BusinessObject.Services.UserService.CashierService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BepKhoiBackend.API.Controllers.UserControllers.CashierControllers
{
    [Route("api/cashiers")]
    [ApiController]
    public class CashierController : ControllerBase
    {
        private readonly ICashierService _cashierService;

        public CashierController(ICashierService cashierService)
        {
            _cashierService = cashierService;
        }

        // Lấy danh sách tất cả Cashier
        [HttpGet]
        public ActionResult<IEnumerable<CashierDTO>> GetAllCashiers()
        {
            var cashiers = _cashierService.GetAllCashiers();
            return Ok(cashiers);
        }

        // Lấy thông tin Cashier theo ID
        [HttpGet("{id}")]
        public ActionResult<CashierDTO> GetCashierById(int id)
        {
            var cashier = _cashierService.GetCashierById(id);
            if (cashier == null)
            {
                return NotFound(new { message = "Cashier not found" });
            }
            return Ok(cashier);
        }
        [HttpPost]
        //[Authorize(Roles = "Admin, Manager")]
        public IActionResult CreateCashier([FromBody] CreateCashierDTO newCashier)
        {
            if (newCashier == null)
            {
                return BadRequest("Dữ liệu không hợp lệ.");
            }

            _cashierService.CreateCashier(newCashier.Email, newCashier.Password, newCashier.Phone, newCashier.UserName);
            return Ok("Cashier đã được tạo thành công.");
        }

        [HttpPut("{id}")]
        //[Authorize(Roles = "Admin, Manager")]
        public IActionResult UpdateCashier(int id, [FromBody] UpdateCashierDTO updatedCashier)
        {
            if (updatedCashier == null)
            {
                return BadRequest("Dữ liệu không hợp lệ.");
            }

            _cashierService.UpdateCashier(id, updatedCashier.Email, updatedCashier.Phone, updatedCashier.UserName);
            return Ok($"Cashier có ID {id} đã được cập nhật thành công.");
        }

        [HttpDelete("{id}")]
        //[Authorize(Roles = "Admin")]
        public IActionResult DeleteCashier(int id)
        {
            _cashierService.DeleteCashier(id);
            return Ok($"Cashier có ID {id} đã bị xóa.");
        }

        [HttpGet("{id}/invoices")]
        public ActionResult<IEnumerable<CashierInvoiceDTO>> GetCashierInvoices(int id)
        {
            var invoices = _cashierService.GetCashierInvoices(id);
            if (invoices == null || !invoices.Any())
            {
                return NotFound($"Không có hóa đơn nào cho cashier với ID {id}.");
            }
            return Ok(new { CashierId = id, Invoices = invoices });
        }

        [HttpGet("search")]
        public ActionResult<List<CashierDTO>> GetCashiersByNameOrPhone([FromQuery] string searchTerm)
        {
            var cashiers = _cashierService.GetCashiersByNameOrPhone(searchTerm);
            return Ok(cashiers);
        }

        [HttpGet("status/{status}")]
        public IActionResult GetCashiersByStatus(bool status)
        {
            var cashiers = _cashierService.GetCashierByStatus(status);
            return Ok(cashiers);
        }

    }
}
