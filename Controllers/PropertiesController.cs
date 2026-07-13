using Hotel_Booking_System.Models.ViewModels;
using Hotel_Booking_System.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel_Booking_System.Controllers
{
    public class PropertiesController : Controller
    {
        private readonly IPropertyService _propertyService;
        private readonly IReviewService _reviewService;

        public PropertiesController(
            IPropertyService propertyService,
            IReviewService reviewService)
        {
            _propertyService = propertyService;
            _reviewService = reviewService;
        }

        // GET: Properties
        public async Task<IActionResult> Index()
        {
            var properties = await _propertyService.GetAllPropertiesAsync();
            return View(properties);
        }
        public async Task<IActionResult> Restaurants()
        {
            var restaurants = await _propertyService.GetPropertiesByTypeAsync("Restaurant");
            return View(restaurants);
        }

        // GET: Properties/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var property = await _propertyService.GetPropertyWithDetailsAsync(id);
            if (property == null)
            {
                return NotFound();
            }

            var reviews = await _reviewService.GetReviewsByPropertyAsync(id);
            ViewBag.Reviews = reviews;
            ViewBag.AverageRating = await _reviewService.GetAverageRatingByPropertyAsync(id);
            ViewBag.CategoryRatings = await _reviewService.GetCategoryRatingsAsync(id);

            return View(property);
        }

        // GET: Properties/Featured
        public async Task<IActionResult> Featured()
        {
            var properties = await _propertyService.GetFeaturedPropertiesAsync(6);
            return PartialView("_FeaturedProperties", properties);
        }

        // GET: Properties/Search
        public async Task<IActionResult> Search(string location, string type, int? guests)
        {
            var properties = await _propertyService.SearchPropertiesAsync(location, type, guests);
            return View("Index", properties);
        }

        // GET: Properties/ByType
        public async Task<IActionResult> ByType(string type)
        {
            var properties = await _propertyService.GetPropertiesByTypeAsync(type);
            ViewBag.Type = type;
            return View("Index", properties);
        }

        // GET: Properties/TopRated
        public async Task<IActionResult> TopRated()
        {
            var properties = await _propertyService.GetTopRatedPropertiesAsync(10);
            return View("Index", properties);
        }

        #region Admin Actions

        // GET: Properties/Admin
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Admin()
        {
            var properties = await _propertyService.GetAllPropertiesAsync();
            return View(properties);
        }

        // GET: Properties/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Properties/Create
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PropertyViewModel model)
        {
            // Remove validation for optional fields
            ModelState.Remove("Reviews");
            ModelState.Remove("RoomTypes");
            ModelState.Remove("Highlights");
            ModelState.Remove("AvailableDates");
            ModelState.Remove("Amenities");
            ModelState.Remove("GalleryImages");

            // Set ThumbnailUrl if empty
            if (string.IsNullOrEmpty(model.ThumbnailUrl))
            {
                model.ThumbnailUrl = model.ImageUrl ?? "/images/default-property.jpg";
            }

            // Process Gallery Images
            if (!string.IsNullOrEmpty(model.GalleryImagesInput))
            {
                model.GalleryImages = model.GalleryImagesInput
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim())
                    .ToList();
            }
            else
            {
                model.GalleryImages = new List<string>();
            }

            // Process Amenities
            if (!string.IsNullOrEmpty(model.AmenitiesInput))
            {
                model.Amenities = model.AmenitiesInput
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim())
                    .ToList();
            }
            else
            {
                model.Amenities = new List<string>();
            }

            // Set default values
            if (string.IsNullOrEmpty(model.PostalCode))
            {
                model.PostalCode = "00000";
            }
            if (model.MinNights == 0) model.MinNights = 1;
            if (model.MaxGuests == 0) model.MaxGuests = 2;
            if (string.IsNullOrEmpty(model.Currency)) model.Currency = "USD";

            if (ModelState.IsValid)
            {
                await _propertyService.CreatePropertyAsync(model);
                TempData["Success"] = "Property added successfully!";
                return RedirectToAction(nameof(Admin));
            }

            return View(model);
        }

        // GET: Properties/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var property = await _propertyService.GetPropertyByIdAsync(id);
            if (property == null)
            {
                return NotFound();
            }
            return View(property);
        }

        // POST: Properties/Edit/5
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PropertyViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            // Remove validation for optional fields
            ModelState.Remove("Reviews");
            ModelState.Remove("RoomTypes");
            ModelState.Remove("Highlights");
            ModelState.Remove("AvailableDates");
            ModelState.Remove("Amenities");
            ModelState.Remove("GalleryImages");

            // Process Gallery Images
            if (!string.IsNullOrEmpty(model.GalleryImagesInput))
            {
                var newImages = model.GalleryImagesInput
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim())
                    .ToList();

                var existingImages = model.GalleryImages ?? new List<string>();
                var allImages = existingImages.ToList();
                allImages.AddRange(newImages);
                model.GalleryImages = allImages;
            }

            // Process Removed Gallery Images
            if (!string.IsNullOrEmpty(model.RemovedGalleryImages))
            {
                var removedImages = model.RemovedGalleryImages
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim())
                    .ToList();

                model.GalleryImages = model.GalleryImages
                    .Where(img => !removedImages.Contains(img))
                    .ToList();
            }

            // Process Amenities
            if (!string.IsNullOrEmpty(model.AmenitiesInput))
            {
                model.Amenities = model.AmenitiesInput
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim())
                    .ToList();
            }

            // Set default values
            if (string.IsNullOrEmpty(model.PostalCode))
            {
                model.PostalCode = "00000";
            }

            if (ModelState.IsValid)
            {
                await _propertyService.UpdatePropertyAsync(model);
                TempData["Success"] = "Property updated successfully!";
                return RedirectToAction(nameof(Admin));
            }
            return View(model);
        }

        // GET: Properties/Delete/5 (Confirmation Page - Optional)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var property = await _propertyService.GetPropertyByIdAsync(id);
            if (property == null)
            {
                return NotFound();
            }
            return View(property);
        }

        // POST: Properties/Delete/5 (With Confirmation Page)
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _propertyService.DeletePropertyAsync(id);
                TempData["Success"] = "Property deleted successfully!";
                return RedirectToAction(nameof(Admin));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error deleting property: {ex.Message}";
                return RedirectToAction(nameof(Admin));
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteDirect(int id)
        {
            try
            {
                await _propertyService.DeletePropertyAsync(id);
                TempData["Success"] = "Property deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error deleting property: {ex.Message}";
            }
            return RedirectToAction(nameof(Admin));
        }

        #endregion
    }
}