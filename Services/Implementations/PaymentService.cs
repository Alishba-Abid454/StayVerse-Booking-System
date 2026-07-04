using Hotel_Booking_System.Models;
using Hotel_Booking_System.Models.Interfaces;
using Hotel_Booking_System.Models.ViewModels;
using Hotel_Booking_System.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel_Booking_System.Services.Implementations
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IReservationRepository _reservationRepository;

        public PaymentService(
            IPaymentRepository paymentRepository,
            IReservationRepository reservationRepository)
        {
            _paymentRepository = paymentRepository;
            _reservationRepository = reservationRepository;
        }

        public async Task<PaymentViewModel> ProcessPaymentAsync(int reservationId, string paymentMethod, decimal amount)
        {
            var reservation = await _reservationRepository.GetReservationWithDetailsAsync(reservationId);
            if (reservation == null)
                throw new Exception("Reservation not found");

            if (reservation.IsPaid)
                throw new Exception("Reservation is already paid");

            // Validate amount
            if (amount != reservation.TotalPrice)
                throw new Exception("Payment amount does not match reservation total");

            // Generate transaction ID
            var transactionId = $"TXN-{DateTime.Now:yyyyMMdd}-{new Random().Next(100000, 999999)}";

            var payment = new Payment
            {
                ReservationId = reservationId,
                TransactionId = transactionId,
                PaymentMethod = paymentMethod,
                Amount = amount,
                Currency = reservation.Currency ?? "USD",
                Status = "Completed", // In real implementation, you'd validate with payment gateway
                PaymentDate = DateTime.UtcNow,
                CardLastFour = "1234", // Masked card number
                CardBrand = "Visa"
            };

            await _paymentRepository.AddAsync(payment);
            await _paymentRepository.SaveChangesAsync();

            // Update reservation as paid
            reservation.IsPaid = true;
            reservation.PaymentMethod = paymentMethod;
            reservation.PaymentDate = DateTime.UtcNow;
            reservation.UpdatedAt = DateTime.UtcNow;

            _reservationRepository.Update(reservation);
            await _reservationRepository.SaveChangesAsync();

            return MapToViewModel(payment);
        }

        public async Task<PaymentViewModel> GetPaymentByIdAsync(int id)
        {
            var payment = await _paymentRepository.GetByIdAsync(id);
            return payment != null ? MapToViewModel(payment) : null;
        }

        public async Task<PaymentViewModel> GetPaymentByTransactionIdAsync(string transactionId)
        {
            var payment = await _paymentRepository.GetPaymentByTransactionIdAsync(transactionId);
            return payment != null ? MapToViewModel(payment) : null;
        }

        public async Task<PaymentViewModel> GetPaymentByReservationIdAsync(int reservationId)
        {
            var payment = await _paymentRepository.GetPaymentByReservationIdAsync(reservationId);
            return payment != null ? MapToViewModel(payment) : null;
        }

        public async Task<IEnumerable<PaymentViewModel>> GetPaymentsByUserAsync(string userId)
        {
            var payments = await _paymentRepository.GetPaymentsByUserAsync(userId);
            return payments.Select(p => MapToViewModel(p));
        }

        public async Task<IEnumerable<PaymentViewModel>> GetPaymentsByStatusAsync(string status)
        {
            var payments = await _paymentRepository.GetPaymentsByStatusAsync(status);
            return payments.Select(p => MapToViewModel(p));
        }

        public async Task<IEnumerable<PaymentViewModel>> GetPaymentsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var payments = await _paymentRepository.GetPaymentsByDateRangeAsync(startDate, endDate);
            return payments.Select(p => MapToViewModel(p));
        }

        public async Task<decimal> GetTotalRevenueAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            return await _paymentRepository.GetTotalRevenueAsync(startDate, endDate);
        }

        public async Task<decimal> GetRevenueByMethodAsync(string paymentMethod, DateTime? startDate = null, DateTime? endDate = null)
        {
            return await _paymentRepository.GetRevenueByMethodAsync(paymentMethod, startDate, endDate);
        }

        public async Task<Dictionary<string, decimal>> GetRevenueByPaymentMethodAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            return await _paymentRepository.GetRevenueByPaymentMethodAsync(startDate, endDate);
        }

        public async Task<bool> RefundPaymentAsync(int paymentId, string reason)
        {
            var payment = await _paymentRepository.GetByIdAsync(paymentId);
            if (payment == null)
                return false;

            if (payment.Status != "Completed")
                return false;

            payment.Status = "Refunded";
            payment.RefundDate = DateTime.UtcNow;
            payment.RefundReason = reason;
            payment.RefundAmount = (int?)payment.Amount;

            _paymentRepository.Update(payment);
            await _paymentRepository.SaveChangesAsync();

            // Update reservation
            var reservation = await _reservationRepository.GetReservationWithDetailsAsync(payment.ReservationId);
            if (reservation != null)
            {
                reservation.IsPaid = false;
                reservation.Status = "Cancelled";
                reservation.UpdatedAt = DateTime.UtcNow;

                _reservationRepository.Update(reservation);
                await _reservationRepository.SaveChangesAsync();
            }

            return true;
        }

        public async Task<bool> IsReservationPaidAsync(int reservationId)
        {
            return await _paymentRepository.IsReservationPaidAsync(reservationId);
        }

        public async Task<decimal> GetTotalRefundedAmountAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            return await _paymentRepository.GetTotalRefundedAmountAsync(startDate, endDate);
        }

        public async Task<int> GetPaymentCountByStatusAsync(string status)
        {
            return await _paymentRepository.GetPaymentCountByStatusAsync(status);
        }

        #region Mapping Methods

        private PaymentViewModel MapToViewModel(Payment payment)
        {
            return new PaymentViewModel
            {
                Id = payment.Id,
                ReservationId = payment.ReservationId,
                ReservationNumber = payment.Reservation?.ReservationNumber,
                TransactionId = payment.TransactionId,
                PaymentMethod = payment.PaymentMethod,
                Amount = payment.Amount,
                Currency = payment.Currency,
                Status = payment.Status,
                CardLastFour = payment.CardLastFour,
                CardBrand = payment.CardBrand,
                PaymentDate = payment.PaymentDate,
                RefundDate = payment.RefundDate,
                RefundReason = payment.RefundReason,
                RefundAmount = payment.RefundAmount
            };
        }

        #endregion
    }
}