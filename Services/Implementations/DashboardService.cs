using Hotel_Booking_System.Models.Interfaces;
using Hotel_Booking_System.Models.ViewModels;
using Hotel_Booking_System.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel_Booking_System.Services.Implementations
{
    public class DashboardService : IDashboardService
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly IPropertyRepository _propertyRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPaymentRepository _paymentRepository;

        public DashboardService(
            IReservationRepository reservationRepository,
            IPropertyRepository propertyRepository,
            IUserRepository userRepository,
            IPaymentRepository paymentRepository)
        {
            _reservationRepository = reservationRepository;
            _propertyRepository = propertyRepository;
            _userRepository = userRepository;
            _paymentRepository = paymentRepository;
        }

        public async Task<DashboardViewModel> GetDashboardStatsAsync()
        {
            var now = DateTime.Now;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);
            var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);
            var startOfYear = new DateTime(now.Year, 1, 1);

            var dashboard = new DashboardViewModel();

            // Basic Stats
            dashboard.TotalProperties = await _propertyRepository.CountAsync();
            dashboard.TotalReservations = await _reservationRepository.CountAsync();
            dashboard.TotalUsers = await _userRepository.GetTotalUsersAsync();
            dashboard.TotalRevenue = (int)await _paymentRepository.GetTotalRevenueAsync();

            // Monthly Revenue
            dashboard.MonthlyRevenue = await _paymentRepository.GetTotalRevenueAsync(startOfMonth, endOfMonth);
            dashboard.YearlyRevenue = await _paymentRepository.GetTotalRevenueAsync(startOfYear, now);

            // Booking Status Counts
            dashboard.ActiveReservations = await _reservationRepository.GetBookingCountByStatusAsync("Confirmed");
            dashboard.PendingReservations = await _reservationRepository.GetBookingCountByStatusAsync("Pending");
            dashboard.CancelledReservations = await _reservationRepository.GetBookingCountByStatusAsync("Cancelled");

            // Average Rating
            var properties = await _propertyRepository.GetAllAsync();
            dashboard.AverageRating = properties.Any() ? properties.Average(p => p.Rating) : 0;

            // Occupancy Rate
            var today = DateTime.Now;
            var thirtyDaysAgo = today.AddDays(-30);
            var occupancyRates = await _propertyRepository.GetOccupancyRatesAsync(thirtyDaysAgo, today);
            dashboard.OccupancyRate = occupancyRates.Any() ? occupancyRates.Values.Average() : 0;

            // Recent Reservations - ✅ FIXED
            var recentReservations = await GetRecentReservationsAsync(5);
            dashboard.RecentReservations = recentReservations.ToList();

            // Top Properties - ✅ FIXED
            var topProperties = await GetTopPerformingPropertiesAsync(5);
            dashboard.TopProperties = topProperties.ToList();

            // Monthly Stats - ✅ FIXED
            var monthlyStats = await GetMonthlyStatsAsync(12);
            dashboard.MonthlyStats = monthlyStats.ToList();

            // Additional Stats
            dashboard.ActiveProperties = await _propertyRepository.CountAsync(p => p.IsActive);
            dashboard.FeaturedProperties = await _propertyRepository.CountAsync(p => p.IsFeatured);

            // Property Type Counts
            var allProperties = await _propertyRepository.GetAllAsync();
            dashboard.HotelsCount = allProperties.Count(p => p.Type == "Hotel");
            dashboard.ApartmentsCount = allProperties.Count(p => p.Type == "Apartment");
            dashboard.RestaurantsCount = allProperties.Count(p => p.Type == "Restaurant");

            return dashboard;
        }

        public async Task<DashboardViewModel> GetDashboardStatsAsync(DateTime startDate, DateTime endDate)
        {
            var dashboard = new DashboardViewModel();

            // Revenue
            dashboard.MonthlyRevenue = await _paymentRepository.GetTotalRevenueAsync(startDate, endDate);

            // Bookings
            var bookings = await _reservationRepository.GetReservationsByDateRangeAsync(startDate, endDate);
            dashboard.TotalReservations = bookings.Count();

            // Active Bookings
            dashboard.ActiveReservations = bookings.Count(r => r.Status == "Confirmed");
            dashboard.PendingReservations = bookings.Count(r => r.Status == "Pending");

            // Guests
            dashboard.TotalGuests = bookings.Sum(r => r.NumberOfGuests);

            return dashboard;
        }

        public async Task<decimal> GetTotalRevenueAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            return await _paymentRepository.GetTotalRevenueAsync(startDate, endDate);
        }

        public async Task<int> GetTotalBookingsAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            if (startDate.HasValue && endDate.HasValue)
            {
                var bookings = await _reservationRepository.GetReservationsByDateRangeAsync(startDate.Value, endDate.Value);
                return bookings.Count();
            }
            return await _reservationRepository.CountAsync();
        }

        public async Task<int> GetActiveBookingsAsync()
        {
            return await _reservationRepository.GetBookingCountByStatusAsync("Confirmed");
        }

        public async Task<int> GetPendingBookingsAsync()
        {
            return await _reservationRepository.GetBookingCountByStatusAsync("Pending");
        }

        public async Task<int> GetTotalUsersAsync()
        {
            return await _userRepository.GetTotalUsersAsync();
        }

        public async Task<int> GetTotalPropertiesAsync()
        {
            return await _propertyRepository.CountAsync();
        }

        public async Task<double> GetOverallOccupancyRateAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var from = startDate ?? DateTime.Now.AddDays(-30);
            var to = endDate ?? DateTime.Now;

            var occupancyRates = await _propertyRepository.GetOccupancyRatesAsync(from, to);
            return occupancyRates.Any() ? occupancyRates.Values.Average() : 0;
        }

        public async Task<decimal> GetAverageDailyRateAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            return await _reservationRepository.GetAverageDailyRateAsync(startDate, endDate);
        }

        public async Task<IEnumerable<RecentReservationViewModel>> GetRecentReservationsAsync(int count = 5)
        {
            var reservations = await _reservationRepository.GetAllAsync();
            var recent = reservations
                .OrderByDescending(r => r.CreatedAt)
                .Take(count)
                .ToList();

            return recent.Select(r => new RecentReservationViewModel
            {
                Id = r.Id,
                GuestName = r.UserName,
                GuestEmail = r.UserEmail,
                PropertyName = r.Property?.Name ?? "Unknown",
                PropertyImage = r.Property?.ImageUrl,
                Status = r.Status,
                Amount = r.TotalPrice,
                Date = r.CreatedAt.ToString("MMM dd, yyyy HH:mm"),
                CheckInDate = r.CheckInDate,
                CheckOutDate = r.CheckOutDate
            });
        }

        public async Task<IEnumerable<PropertyStatsViewModel>> GetTopPerformingPropertiesAsync(int count = 5)
        {
            var properties = await _propertyRepository.GetAllAsync();
            var propertyStats = new List<PropertyStatsViewModel>();

            foreach (var property in properties)
            {
                var bookings = await _reservationRepository.GetReservationsByPropertyAsync(property.Id);
                var completedBookings = bookings.Where(b => b.Status != "Cancelled");

                propertyStats.Add(new PropertyStatsViewModel
                {
                    Id = property.Id,
                    Name = property.Name,
                    ImageUrl = property.ImageUrl,
                    Type = property.Type,
                    Bookings = completedBookings.Count(),
                    Revenue = completedBookings.Sum(b => b.TotalPrice),
                    Rating = property.Rating,
                    OccupancyRate = bookings.Any() ?
                        (double)bookings.Count(b => b.Status == "Confirmed") / bookings.Count() * 100 : 0
                });
            }

            return propertyStats
                .OrderByDescending(p => p.Revenue)
                .Take(count);
        }

        public async Task<IEnumerable<MonthlyStatsViewModel>> GetMonthlyStatsAsync(int months = 12)
        {
            var stats = new List<MonthlyStatsViewModel>();
            var now = DateTime.Now;

            for (int i = months - 1; i >= 0; i--)
            {
                var monthDate = now.AddMonths(-i);
                var startDate = new DateTime(monthDate.Year, monthDate.Month, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);

                var bookings = await _reservationRepository.GetReservationsByDateRangeAsync(startDate, endDate);
                var completedBookings = bookings.Where(b => b.Status != "Cancelled");

                stats.Add(new MonthlyStatsViewModel
                {
                    Month = monthDate.ToString("MMM"),
                    MonthShort = monthDate.ToString("MMM"),
                    MonthNumber = monthDate.Month,
                    Year = monthDate.Year,
                    Bookings = completedBookings.Count(),
                    Revenue = completedBookings.Sum(b => b.TotalPrice),
                    UniqueGuests = completedBookings.Select(b => b.UserId).Distinct().Count(),
                    OccupancyRate = bookings.Any() ?
                        (double)bookings.Count(b => b.Status == "Confirmed") / bookings.Count() * 100 : 0
                });
            }

            return stats;
        }
    }
}