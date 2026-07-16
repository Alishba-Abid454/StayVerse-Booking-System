using Hotel_Booking_System.Models.ViewModels;
using Hotel_Booking_System.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Hotel_Booking_System.Controllers
{
    [Authorize]
    public class PaymentController : Controller
    {
        private readonly IPaymentService _paymentService;
        private readonly IReservationService _reservationService;

        public PaymentController(
            IPaymentService paymentService,
            IReservationService reservationService)
        {
            _paymentService = paymentService;
            _reservationService = reservationService;
        }

        // GET: Payment/Index/{reservationId}
        public async Task<IActionResult> Index(int reservationId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var reservation = await _reservationService.GetReservationByIdAsync(reservationId);

            if (reservation == null)
                return NotFound();

            if (reservation.UserId != userId && !User.IsInRole("Admin"))
            {
                TempData["Error"] = "You don't have permission to view this reservation.";
                return RedirectToAction("MyReservations", "Reservation");
            }

            // Check if already paid
            var existingPayment = await _paymentService.GetPaymentByReservationIdAsync(reservationId);
            if (existingPayment != null && existingPayment.Status == "Completed")
            {
                return RedirectToAction("Success", new { reservationId });
            }

            var model = new PaymentViewModel
            {
                ReservationId = reservation.Id,
                ReservationNumber = reservation.ReservationNumber,
                Amount = reservation.TotalPrice,
                Currency = reservation.Currency ?? "USD",
                Status = "Pending"
            };

            return View(model);
        }

        // ✅ POST: Payment/Process - FIXED
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Process(PaymentViewModel model)
        {
            // ============================================
            // ✅ FIX: Remove validation for display-only fields
            // ============================================
            ModelState.Remove("Status");
            ModelState.Remove("CardBrand");
            ModelState.Remove("CardLastFour");
            ModelState.Remove("TransactionId");
            ModelState.Remove("ReceiptUrl");
            ModelState.Remove("InvoiceNumber");
            ModelState.Remove("RefundReason");
            ModelState.Remove("RefundAmount");
            ModelState.Remove("RefundDate");
            ModelState.Remove("PaymentDate");
            ModelState.Remove("BillingAddress");
            ModelState.Remove("BillingCity");
            ModelState.Remove("BillingCountry");
            ModelState.Remove("BillingPostalCode");
            ModelState.Remove("ReservationNumber");
            ModelState.Remove("Currency");
            ModelState.Remove("Id");

            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // Verify reservation belongs to user
                var reservation = await _reservationService.GetReservationByIdAsync(model.ReservationId);
                if (reservation == null || (reservation.UserId != userId && !User.IsInRole("Admin")))
                {
                    TempData["Error"] = "Invalid reservation.";
                    return RedirectToAction("MyReservations", "Reservation");
                }

                // Process the payment
                var payment = await _paymentService.ProcessPaymentAsync(
                    model.ReservationId,
                    model.PaymentMethod,
                    model.Amount
                );

                return RedirectToAction("Success", new { reservationId = model.ReservationId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View("Index", model);
            }
        }

        // GET: Payment/Success/{reservationId}
        public async Task<IActionResult> Success(int reservationId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var reservation = await _reservationService.GetReservationByIdAsync(reservationId);

            if (reservation == null)
                return NotFound();

            if (reservation.UserId != userId && !User.IsInRole("Admin"))
            {
                TempData["Error"] = "You don't have permission to view this.";
                return RedirectToAction("MyReservations", "Reservation");
            }

            var payment = await _paymentService.GetPaymentByReservationIdAsync(reservationId);

            if (payment == null || payment.Status != "Completed")
            {
                return RedirectToAction("Index", new { reservationId });
            }

            ViewBag.Reservation = reservation;
            return View(payment);
        }

        // GET: Payment/History
        public async Task<IActionResult> History()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var payments = await _paymentService.GetPaymentsByUserAsync(userId);
            return View(payments);
        }
    }
}