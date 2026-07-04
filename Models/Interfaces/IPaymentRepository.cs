namespace Hotel_Booking_System.Models.Interfaces
{
    public interface IPaymentRepository : IRepository<Payment>
    {
        Task<Payment> GetPaymentByTransactionIdAsync(string transactionId);
        Task<Payment> GetPaymentByReservationIdAsync(int reservationId);
        Task<IEnumerable<Payment>> GetPaymentsByUserAsync(string userId);
        Task<IEnumerable<Payment>> GetPaymentsByStatusAsync(string status);
        Task<IEnumerable<Payment>> GetPaymentsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<decimal> GetTotalRevenueAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<decimal> GetRevenueByMethodAsync(string paymentMethod, DateTime? startDate = null, DateTime? endDate = null);
        Task<Dictionary<string, decimal>> GetRevenueByPaymentMethodAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<int> GetPaymentCountByStatusAsync(string status);
        Task<IEnumerable<Payment>> GetFailedPaymentsAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<IEnumerable<Payment>> GetRefundedPaymentsAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<bool> IsReservationPaidAsync(int reservationId);
        Task<decimal> GetTotalRefundedAmountAsync(DateTime? startDate = null, DateTime? endDate = null);
    }
}
