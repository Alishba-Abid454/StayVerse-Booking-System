using Hotel_Booking_System.Data;
using Hotel_Booking_System.Models;
using Hotel_Booking_System.Models.ViewModels;
using Hotel_Booking_System.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel_Booking_System.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IDashboardService _dashboardService;
        private readonly IReservationService _reservationService;
        private readonly IPropertyService _propertyService;
        private readonly IUserService _userService;
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _context;

        public AdminController(
            IDashboardService dashboardService,
            IReservationService reservationService,
            IPropertyService propertyService,
            IUserService userService,
            UserManager<User> userManager,
            ApplicationDbContext context)
        {
            _dashboardService = dashboardService;
            _reservationService = reservationService;
            _propertyService = propertyService;
            _userService = userService;
            _userManager = userManager;
            _context = context;
        }

        // GET: Admin/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            var stats = await _dashboardService.GetDashboardStatsAsync();
            return View(stats);
        }

        // GET: Admin/Properties
        // GET: Admin/Properties
        public async Task<IActionResult> Properties()
        {
            var properties = await _propertyService.GetAllPropertiesAsync();

            // Convert PropertyViewModel to PropertyAdminViewModel
            var viewModels = properties.Select(p => new PropertyAdminViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Type = p.Type,
                Location = p.Location,
                Price = p.Price,
                Rating = p.Rating,
                ImageUrl = p.ImageUrl,
                IsActive = p.IsActive,        // ✅ This exists in PropertyAdminViewModel
                IsFeatured = p.IsFeatured,    // ✅ This exists in PropertyAdminViewModel
                TotalBookings = 0,
                TotalRevenue = 0,
                OccupancyRate = 0,
                AvailableRooms = 0,
                TotalRooms = p.RoomTypes?.Count ?? 0,
                CreatedAt = p.CreatedAt,      // ✅ This exists in PropertyAdminViewModel
                UpdatedAt = p.UpdatedAt,      // ✅ This exists in PropertyAdminViewModel
                Status = p.IsActive ? "Active" : "Inactive"
            }).ToList();

            return View(viewModels);
        }

        // GET: Admin/Reservations
        public async Task<IActionResult> Reservations(string status, DateTime? fromDate, DateTime? toDate)
        {
            var reservations = await _reservationService.GetReservationsByStatusAsync(null);

            // Apply filters
            if (!string.IsNullOrEmpty(status))
            {
                reservations = reservations.Where(r => r.Status == status);
            }

            if (fromDate.HasValue)
            {
                reservations = reservations.Where(r => r.CheckInDate >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                reservations = reservations.Where(r => r.CheckOutDate <= toDate.Value);
            }

            return View(reservations);
        }

        // GET: Admin/Users
        public async Task<IActionResult> Users()
        {
            var users = await _userService.GetActiveUsersAsync();
            return View(users);
        }

        // POST: Admin/ToggleUserStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleUserStatus(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                user.IsActive = !user.IsActive;
                await _userManager.UpdateAsync(user);
                TempData["Success"] = $"User {(user.IsActive ? "activated" : "deactivated")} successfully.";
            }
            return RedirectToAction(nameof(Users));
        }

        // GET: Admin/Reports
        public async Task<IActionResult> Reports()
        {
            var stats = await _dashboardService.GetDashboardStatsAsync();
            return View(stats);
        }

        // GET: Admin/Revenue
        public async Task<IActionResult> Revenue()
        {
            var totalRevenue = await _dashboardService.GetTotalRevenueAsync();
            var monthlyStats = await _dashboardService.GetMonthlyStatsAsync(12);
            ViewBag.TotalRevenue = totalRevenue;
            ViewBag.MonthlyStats = monthlyStats;
            return View();
        }

        // GET: Admin/Analytics
        public async Task<IActionResult> Analytics()
        {
            var stats = await _dashboardService.GetDashboardStatsAsync();
            var topProperties = await _dashboardService.GetTopPerformingPropertiesAsync(5);
            ViewBag.TopProperties = topProperties;
            return View(stats);
        }

        // GET: Admin/ManageTables
        public async Task<IActionResult> ManageTables()
        {
            var tables = await _context.Tables.ToListAsync();
            return View(tables ?? new List<Table>());
        }

        // POST: Admin/SaveTable
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveTable(Table model)
        {
            if (ModelState.IsValid)
            {
                if (model.Id == 0)
                {
                    // Add new table
                    model.IsActive = true;
                    _context.Tables.Add(model);
                    TempData["Success"] = "Table added successfully.";
                }
                else
                {
                    // Update existing table
                    var existing = await _context.Tables.FindAsync(model.Id);
                    if (existing != null)
                    {
                        existing.TableNumber = model.TableNumber;
                        existing.Capacity = model.Capacity;
                        existing.Location = model.Location;
                        _context.Tables.Update(existing);
                        TempData["Success"] = "Table updated successfully.";
                    }
                    else
                    {
                        TempData["Error"] = "Table not found.";
                        return RedirectToAction(nameof(ManageTables));
                    }
                }
                await _context.SaveChangesAsync();
            }
            else
            {
                TempData["Error"] = "Please fill in all required fields.";
            }
            return RedirectToAction(nameof(ManageTables));
        }

        // POST: Admin/DeleteTable
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTable(int id)
        {
            var table = await _context.Tables.FindAsync(id);
            if (table != null)
            {
                _context.Tables.Remove(table);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Table deleted successfully.";
            }
            else
            {
                TempData["Error"] = "Table not found.";
            }
            return RedirectToAction(nameof(ManageTables));
        }

        // GET: Admin/ManageProperties (Alternative view)
        public async Task<IActionResult> ManageProperties()
        {
            var properties = await _propertyService.GetAllPropertiesAsync();
            return View(properties);
        }

        // POST: Admin/TogglePropertyStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TogglePropertyStatus(int id)
        {
            var property = await _propertyService.GetPropertyByIdAsync(id);
            if (property != null)
            {
                property.IsActive = !property.IsActive;
                await _propertyService.UpdatePropertyAsync(property);
                TempData["Success"] = $"Property {(property.IsActive ? "activated" : "deactivated")} successfully.";
            }
            return RedirectToAction(nameof(Properties));
        }

        // GET: Admin/PropertyDetails/5
        public async Task<IActionResult> PropertyDetails(int id)
        {
            var property = await _propertyService.GetPropertyWithDetailsAsync(id);
            if (property == null)
            {
                return NotFound();
            }
            return View(property);
        }
    }
}