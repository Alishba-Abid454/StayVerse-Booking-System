namespace Hotel_Booking_System.Models.Interfaces
{
    public interface IReservationRepository : IRepository<Reservation>
    {
        Task<Reservation> GetReservationWithDetailsAsync(int id);
        Task<Reservation> GetReservationByNumberAsync(string reservationNumber);
        Task<IEnumerable<Reservation>> GetReservationsByUserAsync(string userId);
        Task<IEnumerable<Reservation>> GetReservationsByUserEmailAsync(string email);
        Task<IEnumerable<Reservation>> GetReservationsByPropertyAsync(int propertyId);
        Task<IEnumerable<Reservation>> GetActiveReservationsAsync();
        Task<IEnumerable<Reservation>> GetUpcomingReservationsAsync(int days = 7);
        Task<IEnumerable<Reservation>> GetReservationsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<Reservation>> GetReservationsByStatusAsync(string status);
        Task<bool> IsPropertyAvailableAsync(int propertyId, DateTime checkIn, DateTime checkOut, int? roomTypeId = null);
        Task<bool> IsRoomTypeAvailableAsync(int roomTypeId, DateTime checkIn, DateTime checkOut, int quantity = 1);
        Task<IEnumerable<DateTime>> GetAvailableDatesAsync(int propertyId, int? roomTypeId = null);
        Task<decimal> GetTotalRevenueAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<Dictionary<string, decimal>> GetRevenueByPropertyAsync(DateTime startDate, DateTime endDate);
        Task<int> GetBookingCountByStatusAsync(string status);
        Task<int> GetTotalGuestsAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<decimal> GetAverageDailyRateAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<double> GetOccupancyRateAsync(int propertyId, DateTime startDate, DateTime endDate);
        Task<IEnumerable<Reservation>> GetCancelledReservationsAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<int> GetNoShowCountAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<string> GenerateReservationNumberAsync();
    }
}
