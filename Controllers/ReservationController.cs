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
        private readonly UserManager<User> _userManager;

        public ReservationController(
            IReservationService reservationService,
            IPropertyService propertyService,
            IRoomTypeService roomTypeService,
            UserManager<User> userManager)
        {
            _reservationService = reservationService;
            _propertyService = propertyService;
            _roomTypeService = roomTypeService;
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

            return View(reservation);
        }

        // GET: Reservation/Book/5
        public async Task<IActionResult> Book(int propertyId)
        {
            var property = await _propertyService.GetPropertyWithDetailsAsync(propertyId);
            if (property == null)
            {
                return NotFound();
            }

            var model = new BookingViewModel
            {
                PropertyId = propertyId,
                PropertyName = property.Name,
                PropertyImage = property.ImageUrl,
                PropertyType = property.Type,
                PropertyLocation = property.Location,
                CheckInDate = DateTime.Now.AddDays(1),
                CheckOutDate = DateTime.Now.AddDays(3),
                NumberOfGuests = 2,
                NumberOfRooms = 1,
                PricePerNight = property.BasePrice,
                RoomTypes = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>(),
                AvailableRooms = property.RoomTypes ?? new List<RoomTypeViewModel>()
            };

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

            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                model.FullName = $"{user.FirstName} {user.LastName}".Trim();
                model.Email = user.Email;
                model.PhoneNumber = user.PhoneNumber;
                model.Country = user.Country;
                model.City = user.City;
                model.Address = user.Address;
            }

            return View(model);
        }

        // POST: Reservation/Book
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Book(BookingViewModel model)
        {
            // Validate room type selection
            if (!model.SelectedRoomTypeId.HasValue || model.SelectedRoomTypeId.Value <= 0)
            {
                ModelState.AddModelError("SelectedRoomTypeId", "Please select a room type.");
            }

            if (!ModelState.IsValid)
            {
                var property = await _propertyService.GetPropertyWithDetailsAsync(model.PropertyId);
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
                var reservation = await _reservationService.CreateReservationAsync(model, userId);
                return RedirectToAction(nameof(Confirmation), new { id = reservation.Id });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);

                var property = await _propertyService.GetPropertyWithDetailsAsync(model.PropertyId);
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

            // Check if cancellation is allowed
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