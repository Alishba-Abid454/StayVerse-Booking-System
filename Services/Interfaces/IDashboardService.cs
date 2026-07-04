using Hotel_Booking_System.Models.ViewModels;
using System.Threading.Tasks;

namespace Hotel_Booking_System.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardViewModel> GetDashboardStatsAsync();
        Task<DashboardViewModel> GetDashboardStatsAsync(DateTime startDate, DateTime endDate);
        Task<decimal> GetTotalRevenueAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<int> GetTotalBookingsAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<int> GetActiveBookingsAsync();
        Task<int> GetPendingBookingsAsync();
        Task<int> GetTotalUsersAsync();
        Task<int> GetTotalPropertiesAsync();
        Task<double> GetOverallOccupancyRateAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<decimal> GetAverageDailyRateAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<IEnumerable<RecentReservationViewModel>> GetRecentReservationsAsync(int count = 5);
        Task<IEnumerable<PropertyStatsViewModel>> GetTopPerformingPropertiesAsync(int count = 5);
        Task<IEnumerable<MonthlyStatsViewModel>> GetMonthlyStatsAsync(int months = 12);
    }
}