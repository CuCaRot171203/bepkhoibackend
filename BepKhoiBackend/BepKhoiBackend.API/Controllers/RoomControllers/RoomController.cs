using Microsoft.AspNetCore.Mvc;

namespace BepKhoiBackend.API.Controllers.RoomControllers
{
    public class RoomController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
