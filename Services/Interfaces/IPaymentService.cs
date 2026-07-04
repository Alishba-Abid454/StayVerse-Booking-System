using Hotel_Booking_System.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hotel_Booking_System.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<PaymentViewModel> ProcessPaymentAsync(int reservationId, string paymentMethod, decimal amount);
        Task<PaymentViewModel> GetPaymentByIdAsync(int id);
        Task<PaymentViewModel> GetPaymentByTransactionIdAsync(string transactionId);
        Task<PaymentViewModel> GetPaymentByReservationIdAsync(int reservationId);
        Task<IEnumerable<PaymentViewModel>> GetPaymentsByUserAsync(string userId);
        Task<IEnumerable<PaymentViewModel>> GetPaymentsByStatusAsync(string status);
        Task<IEnumerable<PaymentViewModel>> GetPaymentsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<decimal> GetTotalRevenueAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<decimal> GetRevenueByMethodAsync(string paymentMethod, DateTime? startDate = null, DateTime? endDate = null);
        Task<Dictionary<string, decimal>> GetRevenueByPaymentMethodAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<bool> RefundPaymentAsync(int paymentId, string reason);
        Task<bool> IsReservationPaidAsync(int reservationId);
        Task<decimal> GetTotalRefundedAmountAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<int> GetPaymentCountByStatusAsync(string status);
    }
}