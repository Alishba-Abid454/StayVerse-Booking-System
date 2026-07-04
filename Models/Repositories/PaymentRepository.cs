using Hotel_Booking_System.Data;
using Hotel_Booking_System.Models.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Hotel_Booking_System.Models.Repositories
{
    public class PaymentRepository : Repository<Payment>, IPaymentRepository
    {
        public PaymentRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Payment> GetPaymentByTransactionIdAsync(string transactionId)
        {
            return await _context.Payments
                .FirstOrDefaultAsync(p => p.TransactionId == transactionId);
        }

        public async Task<Payment> GetPaymentByReservationIdAsync(int reservationId)
        {
            return await _context.Payments
                .FirstOrDefaultAsync(p => p.ReservationId == reservationId);
        }

        public async Task<IEnumerable<Payment>> GetPaymentsByUserAsync(string userId)
        {
            return await _context.Payments
                .Include(p => p.Reservation)
                .Where(p => p.Reservation.UserId == userId)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Payment>> GetPaymentsByStatusAsync(string status)
        {
            return await _context.Payments
                .Where(p => p.Status == status)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Payment>> GetPaymentsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Payments
                .Where(p => p.PaymentDate >= startDate && p.PaymentDate <= endDate)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalRevenueAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.Payments
                .Where(p => p.Status == "Completed");

            if (startDate.HasValue)
                query = query.Where(p => p.PaymentDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(p => p.PaymentDate <= endDate.Value);

            return await query.SumAsync(p => p.Amount);
        }

        public async Task<decimal> GetRevenueByMethodAsync(string paymentMethod, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.Payments
                .Where(p => p.PaymentMethod == paymentMethod && p.Status == "Completed");

            if (startDate.HasValue)
                query = query.Where(p => p.PaymentDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(p => p.PaymentDate <= endDate.Value);

            return await query.SumAsync(p => p.Amount);
        }

        public async Task<Dictionary<string, decimal>> GetRevenueByPaymentMethodAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.Payments
                .Where(p => p.Status == "Completed");

            if (startDate.HasValue)
                query = query.Where(p => p.PaymentDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(p => p.PaymentDate <= endDate.Value);

            return await query
                .GroupBy(p => p.PaymentMethod)
                .Select(g => new { Method = g.Key, Revenue = g.Sum(p => p.Amount) })
                .ToDictionaryAsync(x => x.Method, x => x.Revenue);
        }

        public async Task<int> GetPaymentCountByStatusAsync(string status)
        {
            return await _context.Payments
                .CountAsync(p => p.Status == status);
        }

        public async Task<IEnumerable<Payment>> GetFailedPaymentsAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.Payments
                .Where(p => p.Status == "Failed");

            if (startDate.HasValue)
                query = query.Where(p => p.PaymentDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(p => p.PaymentDate <= endDate.Value);

            return await query.OrderByDescending(p => p.PaymentDate).ToListAsync();
        }

        public async Task<IEnumerable<Payment>> GetRefundedPaymentsAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.Payments
                .Where(p => p.Status == "Refunded");

            if (startDate.HasValue)
                query = query.Where(p => p.RefundDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(p => p.RefundDate <= endDate.Value);

            return await query.OrderByDescending(p => p.RefundDate).ToListAsync();
        }

        public async Task<bool> IsReservationPaidAsync(int reservationId)
        {
            return await _context.Payments
                .AnyAsync(p => p.ReservationId == reservationId && p.Status == "Completed");
        }

        public async Task<decimal> GetTotalRefundedAmountAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.Payments
                .Where(p => p.Status == "Refunded");

            if (startDate.HasValue)
                query = query.Where(p => p.RefundDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(p => p.RefundDate <= endDate.Value);

            return await query.SumAsync(p => p.RefundAmount ?? 0);
        }
    }
}
