namespace Hotel_Booking_System.Models.ViewModels
{
    public class DashboardViewModel
    {
        // Summary Statistics
        public int TotalProperties { get; set; }
        public int TotalReservations { get; set; }
        public int TotalUsers { get; set; }
        public int TotalRevenue { get; set; }
        public decimal MonthlyRevenue { get; set; }
        public decimal YearlyRevenue { get; set; }
        public int ActiveReservations { get; set; }
        public int PendingReservations { get; set; }
        public int CancelledReservations { get; set; }
        public double AverageRating { get; set; }
        public double OccupancyRate { get; set; }

        // Property Statistics
        public int ActiveProperties { get; set; }
        public int FeaturedProperties { get; set; }
        public int HotelsCount { get; set; }
        public int ApartmentsCount { get; set; }
        public int RestaurantsCount { get; set; }

        // Revenue Stats
        public decimal TodayRevenue { get; set; }
        public decimal WeekRevenue { get; set; }
        public decimal MonthRevenue { get; set; }
        public decimal YearRevenue { get; set; }
        public decimal PercentageChange { get; set; }

        // Recent Data
        public List<RecentReservationViewModel> RecentReservations { get; set; }
        public List<PropertyStatsViewModel> TopProperties { get; set; }
        public List<MonthlyStatsViewModel> MonthlyStats { get; set; }
        public List<RecentUserViewModel> RecentUsers { get; set; }
        public List<ActivityLogViewModel> RecentActivities { get; set; }
        public int TotalGuests { get; internal set; }
    }

    public class RecentReservationViewModel
    {
        public int Id { get; set; }
        public string GuestName { get; set; }
        public string GuestEmail { get; set; }
        public string PropertyName { get; set; }
        public string PropertyImage { get; set; }
        public string Status { get; set; }
        public decimal Amount { get; set; }
        public string Date { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int Nights => (CheckOutDate - CheckInDate).Days;
    }

    public class PropertyStatsViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public string Type { get; set; }
        public int Bookings { get; set; }
        public decimal Revenue { get; set; }
        public double Rating { get; set; }
        public double OccupancyRate { get; set; }
    }

    public class MonthlyStatsViewModel
    {
        public string Month { get; set; }
        public string MonthShort { get; set; }
        public int MonthNumber { get; set; }
        public int Year { get; set; }
        public int Bookings { get; set; }
        public decimal Revenue { get; set; }
        public int UniqueGuests { get; set; }
        public double OccupancyRate { get; set; }
    }

    public class RecentUserViewModel
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string ProfileImageUrl { get; set; }
        public DateTime JoinedDate { get; set; }
        public int TotalBookings { get; set; }
        public string Status { get; set; }
    }

    public class ActivityLogViewModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Action { get; set; }
        public string Details { get; set; }
        public DateTime Timestamp { get; set; }
        public string Icon { get; set; }
        public string Color { get; set; }
    }
}
