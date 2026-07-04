using Hotel_Booking_System.Models.ViewModels;

namespace Hotel_Booking_System.Services.Interfaces
{
    public interface IReservationService
    {
        Task<ReservationViewModel> CreateReservationAsync(BookingViewModel model, string userId);
        Task<ReservationViewModel> GetReservationByIdAsync(int id);
        Task<ReservationViewModel> GetReservationByNumberAsync(string reservationNumber);
        Task<IEnumerable<ReservationViewModel>> GetUserReservationsAsync(string userId);
        Task<IEnumerable<ReservationViewModel>> GetUserReservationsByEmailAsync(string email);
        Task<IEnumerable<ReservationViewModel>> GetPropertyReservationsAsync(int propertyId);
        Task<IEnumerable<ReservationViewModel>> GetActiveReservationsAsync();
        Task<IEnumerable<ReservationViewModel>> GetUpcomingReservationsAsync(int days = 7);
        Task<IEnumerable<ReservationViewModel>> GetReservationsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<ReservationViewModel>> GetReservationsByStatusAsync(string status);
        Task<bool> CancelReservationAsync(int reservationId, string reason);
        Task<bool> CheckAvailabilityAsync(int propertyId, int? roomTypeId, DateTime checkIn, DateTime checkOut);
        Task<bool> IsRoomTypeAvailableAsync(int roomTypeId, DateTime checkIn, DateTime checkOut, int quantity = 1);
        Task<IEnumerable<DateTime>> GetAvailableDatesAsync(int propertyId, int? roomTypeId = null);
        Task<decimal> GetTotalRevenueAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<Dictionary<string, decimal>> GetRevenueByPropertyAsync(DateTime startDate, DateTime endDate);
        Task<int> GetBookingCountByStatusAsync(string status);
        Task<int> GetTotalGuestsAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<decimal> GetAverageDailyRateAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<double> GetOccupancyRateAsync(int propertyId, DateTime startDate, DateTime endDate);
        Task UpdateReservationStatusAsync(int reservationId, string status);
        Task<string> GenerateReservationNumberAsync();
        Task<int> GetTotalReservationsCountAsync();
    }
}
