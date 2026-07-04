using Hotel_Booking_System.Models.ViewModels;
using Hotel_Booking_System.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Hotel_Booking_System.Controllers
{
    public class HomeController : Controller
    {
        private readonly IPropertyService _propertyService;
        private readonly IReviewService _reviewService;

        public HomeController(
            IPropertyService propertyService,
            IReviewService reviewService)
        {
            _propertyService = propertyService;
            _reviewService = reviewService;
        }

        // GET: Home/Index
        public async Task<IActionResult> Index()
        {
            var featuredProperties = await _propertyService.GetFeaturedPropertiesAsync(6);
            var topRatedProperties = await _propertyService.GetTopRatedPropertiesAsync(4);
            var recentReviews = await _reviewService.GetRecentReviewsAsync(3);

            ViewBag.FeaturedProperties = featuredProperties;
            ViewBag.TopRatedProperties = topRatedProperties;
            ViewBag.RecentReviews = recentReviews;

            return View();
        }

        // GET: Home/About
        public IActionResult About()
        {
            return View();
        }

        // GET: Home/Contact
        public IActionResult Contact()
        {
            return View();
        }

        // GET: Home/Privacy
        public IActionResult Privacy()
        {
            return View();
        }

        // GET: Home/Error
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}