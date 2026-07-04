using Microsoft.AspNetCore.Mvc;

namespace Hotel_Booking_System.Controllers
{
    public class ChatController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
