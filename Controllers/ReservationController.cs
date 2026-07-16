using Hotel_Booking_System.Models;
using Hotel_Booking_System.Models.ViewModels;
using Hotel_Booking_System.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Hotel_Booking_System.Controllers
{
    [Authorize]
    public class ReservationController : Controller
    {
        private readonly IReservationService _reservationService;
        private readonly IPropertyService _propertyService;
        private readonly IRoomTypeService _roomTypeService;
        private readonly IPaymentService _paymentService;
        private readonly UserManager<User> _userManager;

        public ReservationController(
            IReservationService reservationService,
            IPropertyService propertyService,
            IRoomTypeService roomTypeService,
            IPaymentService paymentService,
            UserManager<User> userManager)
        {
            _reservationService = reservationService;
            _propertyService = propertyService;
            _roomTypeService = roomTypeService;
            _paymentService = paymentService;
            _userManager = userManager;
        }

        // GET: Reservation
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var reservations = await _reservationService.GetUserReservationsAsync(userId);
            return View(reservations);
        }

        // GET: Reservation/MyReservations
        public async Task<IActionResult> MyReservations()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var reservations = await _reservationService.GetUserReservationsAsync(userId);
            return View("Index", reservations);
        }

        // GET: Reservation/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var reservation = await _reservationService.GetReservationByIdAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (reservation.UserId != userId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            // Check if payment exists
            var payment = await _paymentService.GetPaymentByReservationIdAsync(id);
            ViewBag.Payment = payment;

            return View(reservation);
        }

        [AllowAnonymous]
        // GET: Reservation/Book
        public async Task<IActionResult> Book(
            int propertyId,
            string? checkin = null,
            string? checkout = null,
            int adults = 2,
            int children = 0,
            int rooms = 1)
        {
            var property = await _propertyService.GetPropertyWithDetailsAsync(propertyId);
            if (property == null)
            {
                return NotFound();
            }

            // ✅ Calculate total guests
            int totalGuests = adults + children;

            // ✅ Validate guests against property max guests
            if (property.MaxGuests > 0 && totalGuests > property.MaxGuests)
            {
                TempData["Error"] = $"This property accepts a maximum of {property.MaxGuests} guest(s). You selected {totalGuests} guest(s).";
                return RedirectToAction("Details", "Properties", new { id = propertyId });
            }

            // ✅ Parse dates from query parameters or use defaults
            DateTime checkInDate;
            DateTime checkOutDate;

            if (!string.IsNullOrEmpty(checkin) && DateTime.TryParse(checkin, out var parsedCheckIn))
            {
                checkInDate = parsedCheckIn;
            }
            else
            {
                checkInDate = DateTime.Now.AddDays(1);
            }

            if (!string.IsNullOrEmpty(checkout) && DateTime.TryParse(checkout, out var parsedCheckOut))
            {
                checkOutDate = parsedCheckOut;
            }
            else
            {
                checkOutDate = checkInDate.AddDays(1);
            }

            // ✅ Ensure check-out is after check-in
            if (checkOutDate <= checkInDate)
            {
                checkOutDate = checkInDate.AddDays(1);
            }

            var model = new BookingViewModel
            {
                PropertyId = propertyId,
                PropertyName = property.Name,
                PropertyImage = property.ImageUrl,
                PropertyType = property.Type,
                PropertyLocation = property.Location,

                // ✅ Use values from query parameters
                CheckInDate = checkInDate,
                CheckOutDate = checkOutDate,
                NumberOfGuests = totalGuests, // ✅ Total guests (adults + children)
                NumberOfRooms = rooms > 0 ? rooms : 1,
                PricePerNight = property.BasePrice,
                RoomTypes = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>(),
                AvailableRooms = property.RoomTypes ?? new List<RoomTypeViewModel>()
            };

            // Populate room types
            if (property.RoomTypes != null && property.RoomTypes.Count > 0)
            {
                foreach (var room in property.RoomTypes)
                {
                    model.RoomTypes.Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                    {
                        Value = room.Id.ToString(),
                        Text = $"{room.Name} - ${room.PricePerNight}/night (Max {room.MaxGuests} guests)"
                    });
                }
                model.SelectedRoomTypeId = property.RoomTypes.First().Id;
            }

            // Pre-populate user info
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    model.FullName = $"{user.FirstName} {user.LastName}".Trim();
                    model.Email = user.Email;
                    model.PhoneNumber = user.PhoneNumber;
                    model.Country = user.Country;
                    model.City = user.City;
                    model.Address = user.Address;
                }
            }

            return View(model);
        }

        // POST: Reservation/Book
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Book(BookingViewModel model)
        {
            // Get property to validate guests
            var property = await _propertyService.GetPropertyWithDetailsAsync(model.PropertyId);
            if (property == null)
            {
                ModelState.AddModelError("", "Property not found.");
                return View(model);
            }

            // ✅ Validate total guests
            if (property.MaxGuests > 0 && model.NumberOfGuests > property.MaxGuests)
            {
                ModelState.AddModelError("", $"This property accepts a maximum of {property.MaxGuests} guest(s). You selected {model.NumberOfGuests} guest(s).");
            }

            // Remove validation for display-only fields
            ModelState.Remove("RoomTypes");
            ModelState.Remove("AvailableRooms");

            // Validate required fields
            if (string.IsNullOrWhiteSpace(model.FullName))
                ModelState.AddModelError(nameof(model.FullName), "Full Name is required.");

            if (string.IsNullOrWhiteSpace(model.Email))
                ModelState.AddModelError(nameof(model.Email), "Email is required.");

            if (string.IsNullOrWhiteSpace(model.PhoneNumber))
                ModelState.AddModelError(nameof(model.PhoneNumber), "Phone Number is required.");

            if (!model.SelectedRoomTypeId.HasValue || model.SelectedRoomTypeId <= 0)
                ModelState.AddModelError(nameof(model.SelectedRoomTypeId), "Please select a room type.");

            if (model.CheckOutDate <= model.CheckInDate)
                ModelState.AddModelError(nameof(model.CheckOutDate), "Check-out date must be after check-in date.");

            if (!ModelState.IsValid)
            {
                // Reload property data
                if (property != null)
                {
                    model.PropertyName = property.Name;
                    model.PropertyImage = property.ImageUrl;
                    model.PropertyType = property.Type;
                    model.PropertyLocation = property.Location;
                    model.PricePerNight = property.BasePrice;
                    model.AvailableRooms = property.RoomTypes ?? new List<RoomTypeViewModel>();
                    model.RoomTypes = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();

                    if (property.RoomTypes != null)
                    {
                        foreach (var room in property.RoomTypes)
                        {
                            model.RoomTypes.Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                            {
                                Value = room.Id.ToString(),
                                Text = $"{room.Name} - ${room.PricePerNight}/night (Max {room.MaxGuests} guests)"
                            });
                        }
                    }
                }
                return View(model);
            }

            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // Create reservation
                var reservation = await _reservationService.CreateReservationAsync(model, userId);

                // Redirect to Payment page
                return RedirectToAction("Index", "Payment", new { reservationId = reservation.Id });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error creating reservation: {ex.Message}");

                if (property != null)
                {
                    model.PropertyName = property.Name;
                    model.PropertyImage = property.ImageUrl;
                    model.PropertyType = property.Type;
                    model.PropertyLocation = property.Location;
                    model.PricePerNight = property.BasePrice;
                    model.AvailableRooms = property.RoomTypes ?? new List<RoomTypeViewModel>();
                    model.RoomTypes = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();

                    if (property.RoomTypes != null)
                    {
                        foreach (var room in property.RoomTypes)
                        {
                            model.RoomTypes.Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                            {
                                Value = room.Id.ToString(),
                                Text = $"{room.Name} - ${room.PricePerNight}/night (Max {room.MaxGuests} guests)"
                            });
                        }
                    }
                }
                return View(model);
            }
        }

        // GET: Reservation/Confirmation/5
        public async Task<IActionResult> Confirmation(int id)
        {
            var reservation = await _reservationService.GetReservationByIdAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (reservation.UserId != userId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            // Check if payment exists and is completed
            var payment = await _paymentService.GetPaymentByReservationIdAsync(id);
            if (payment != null && payment.Status == "Completed")
            {
                ViewBag.Payment = payment;
                ViewBag.IsPaid = true;
            }
            else
            {
                ViewBag.IsPaid = false;
            }

            return View(reservation);
        }

        // GET: Reservation/Cancel/5
        public async Task<IActionResult> Cancel(int id)
        {
            var reservation = await _reservationService.GetReservationByIdAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (reservation.UserId != userId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            if (!reservation.CanCancel)
            {
                TempData["Error"] = "This reservation cannot be cancelled because it's within 24 hours of check-in or already checked out.";
                return RedirectToAction(nameof(Details), new { id });
            }

            return View(reservation);
        }

        // POST: Reservation/Cancel
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelPost(int id, string reason)
        {
            try
            {
                var result = await _reservationService.CancelReservationAsync(id, reason);
                if (!result)
                {
                    TempData["Error"] = "Unable to cancel reservation. Please contact support.";
                    return RedirectToAction(nameof(Details), new { id });
                }

                TempData["Success"] = "Reservation cancelled successfully.";
                return RedirectToAction(nameof(MyReservations));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        // GET: Reservation/CheckAvailability
        public async Task<IActionResult> CheckAvailability(int propertyId, int? roomTypeId, DateTime checkIn, DateTime checkOut)
        {
            var isAvailable = await _reservationService.CheckAvailabilityAsync(propertyId, roomTypeId, checkIn, checkOut);
            return Json(new { available = isAvailable });
        }

        // GET: Reservation/GetAvailableDates
        public async Task<IActionResult> GetAvailableDates(int propertyId, int? roomTypeId = null)
        {
            var dates = await _reservationService.GetAvailableDatesAsync(propertyId, roomTypeId);
            return Json(dates);
        }

        #region Admin Actions

        // GET: Reservation/Admin
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Admin()
        {
            var reservations = await _reservationService.GetReservationsByStatusAsync(null);
            return View(reservations);
        }

        // GET: Reservation/AdminDetails/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminDetails(int id)
        {
            var reservation = await _reservationService.GetReservationByIdAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }
            return View(reservation);
        }

        // POST: Reservation/Admin/UpdateStatus
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            await _reservationService.UpdateReservationStatusAsync(id, status);
            return RedirectToAction(nameof(AdminDetails), new { id });
        }

        #endregion
    }
}
