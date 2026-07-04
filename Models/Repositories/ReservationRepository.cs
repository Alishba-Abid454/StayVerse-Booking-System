using Hotel_Booking_System.Data;
using Hotel_Booking_System.Models.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Hotel_Booking_System.Models.Repositories
{
    public class ReservationRepository : Repository<Reservation>, IReservationRepository
    {
        public ReservationRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Reservation> GetReservationWithDetailsAsync(int id)
        {
            return await _context.Reservations
                .Include(r => r.Property)
                .Include(r => r.RoomType)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<Reservation> GetReservationByNumberAsync(string reservationNumber)
        {
            return await _context.Reservations
                .Include(r => r.Property)
                .Include(r => r.RoomType)
                .FirstOrDefaultAsync(r => r.ReservationNumber == reservationNumber);
        }

        public async Task<IEnumerable<Reservation>> GetReservationsByUserAsync(string userId)
        {
            return await _context.Reservations
                .Include(r => r.Property)
                .Include(r => r.RoomType)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Reservation>> GetReservationsByUserEmailAsync(string email)
        {
            return await _context.Reservations
                .Include(r => r.Property)
                .Include(r => r.RoomType)
                .Where(r => r.UserEmail == email)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Reservation>> GetReservationsByPropertyAsync(int propertyId)
        {
            return await _context.Reservations
                .Include(r => r.RoomType)
                .Where(r => r.PropertyId == propertyId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Reservation>> GetActiveReservationsAsync()
        {
            return await _context.Reservations
                .Include(r => r.Property)
                .Include(r => r.RoomType)
                .Where(r => r.Status == "Confirmed" && r.CheckInDate >= DateTime.Now)
                .OrderBy(r => r.CheckInDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Reservation>> GetUpcomingReservationsAsync(int days = 7)
        {
            var startDate = DateTime.Now;
            var endDate = DateTime.Now.AddDays(days);

            return await _context.Reservations
                .Include(r => r.Property)
                .Include(r => r.RoomType)
                .Where(r => r.Status == "Confirmed" &&
                           r.CheckInDate >= startDate &&
                           r.CheckInDate <= endDate)
                .OrderBy(r => r.CheckInDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Reservation>> GetReservationsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Reservations
                .Include(r => r.Property)
                .Include(r => r.RoomType)
                .Where(r => r.CheckInDate >= startDate && r.CheckOutDate <= endDate)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Reservation>> GetReservationsByStatusAsync(string status)
        {
            return await _context.Reservations
                .Include(r => r.Property)
                .Include(r => r.RoomType)
                .Where(r => r.Status == status)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> IsPropertyAvailableAsync(int propertyId, DateTime checkIn, DateTime checkOut, int? roomTypeId = null)
        {
            var query = _context.Reservations
                .Where(r => r.PropertyId == propertyId &&
                           r.Status != "Cancelled" &&
                           r.CheckInDate < checkOut &&
                           r.CheckOutDate > checkIn);

            if (roomTypeId.HasValue)
            {
                query = query.Where(r => r.RoomTypeId == roomTypeId.Value);
            }

            var count = await query.CountAsync();
            return count == 0;
        }

        public async Task<bool> IsRoomTypeAvailableAsync(int roomTypeId, DateTime checkIn, DateTime checkOut, int quantity = 1)
        {
            var overlapping = await _context.Reservations
                .Where(r => r.RoomTypeId == roomTypeId &&
                           r.Status != "Cancelled" &&
                           r.CheckInDate < checkOut &&
                           r.CheckOutDate > checkIn)
                .CountAsync();

            var roomType = await _context.RoomTypes.FindAsync(roomTypeId);
            if (roomType == null)
                return false;

            return (overlapping + quantity) <= roomType.AvailableQuantity;
        }

        public async Task<IEnumerable<DateTime>> GetAvailableDatesAsync(int propertyId, int? roomTypeId = null)
        {
            var startDate = DateTime.Now.Date;
            var endDate = DateTime.Now.AddMonths(3).Date;
            var availableDates = new List<DateTime>();

            for (var date = startDate; date <= endDate; date = date.AddDays(1))
            {
                var isAvailable = await IsPropertyAvailableAsync(propertyId, date, date.AddDays(1), roomTypeId);
                if (isAvailable)
                {
                    availableDates.Add(date);
                }
            }

            return availableDates;
        }

        public async Task<decimal> GetTotalRevenueAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.Reservations
                .Where(r => r.Status != "Cancelled" && r.IsPaid);

            if (startDate.HasValue)
                query = query.Where(r => r.CheckInDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(r => r.CheckOutDate <= endDate.Value);

            return await query.SumAsync(r => r.TotalPrice);
        }

        public async Task<Dictionary<string, decimal>> GetRevenueByPropertyAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Reservations
                .Where(r => r.Status != "Cancelled" &&
                           r.IsPaid &&
                           r.CheckInDate >= startDate &&
                           r.CheckOutDate <= endDate)
                .GroupBy(r => r.PropertyId)
                .Select(g => new { PropertyId = g.Key, Revenue = g.Sum(r => r.TotalPrice) })
                .ToDictionaryAsync(x => x.PropertyId.ToString(), x => x.Revenue);
        }

        public async Task<int> GetBookingCountByStatusAsync(string status)
        {
            return await _context.Reservations
                .CountAsync(r => r.Status == status);
        }

        public async Task<int> GetTotalGuestsAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.Reservations
                .Where(r => r.Status != "Cancelled");

            if (startDate.HasValue)
                query = query.Where(r => r.CheckInDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(r => r.CheckOutDate <= endDate.Value);

            return await query.SumAsync(r => r.NumberOfGuests);
        }

        public async Task<decimal> GetAverageDailyRateAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.Reservations
                .Where(r => r.Status != "Cancelled" && r.IsPaid);

            if (startDate.HasValue)
                query = query.Where(r => r.CheckInDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(r => r.CheckOutDate <= endDate.Value);

            var totalRevenue = await query.SumAsync(r => r.TotalPrice);
            var totalNights = await query.SumAsync(r => (r.CheckOutDate - r.CheckInDate).Days);

            return totalNights > 0 ? totalRevenue / totalNights : 0;
        }

        public async Task<double> GetOccupancyRateAsync(int propertyId, DateTime startDate, DateTime endDate)
        {
            var totalNights = (endDate - startDate).Days;
            var roomTypes = await _context.RoomTypes
                .Where(r => r.PropertyId == propertyId)
                .SumAsync(r => r.Quantity);

            var totalRoomNights = totalNights * roomTypes;

            var bookedNights = await _context.Reservations
                .Where(r => r.PropertyId == propertyId &&
                           r.Status != "Cancelled" &&
                           r.CheckInDate < endDate &&
                           r.CheckOutDate > startDate)
                .SumAsync(r => (r.CheckOutDate - r.CheckInDate).Days * r.NumberOfRooms);

            return totalRoomNights > 0 ? (double)bookedNights / totalRoomNights * 100 : 0;
        }

        public async Task<IEnumerable<Reservation>> GetCancelledReservationsAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.Reservations
                .Include(r => r.Property)
                .Where(r => r.Status == "Cancelled");

            if (startDate.HasValue)
                query = query.Where(r => r.CancelledAt >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(r => r.CancelledAt <= endDate.Value);

            return await query.OrderByDescending(r => r.CancelledAt).ToListAsync();
        }

        public async Task<int> GetNoShowCountAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.Reservations
                .Where(r => r.Status == "NoShow");

            if (startDate.HasValue)
                query = query.Where(r => r.CheckInDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(r => r.CheckInDate <= endDate.Value);

            return await query.CountAsync();
        }

        public async Task<string> GenerateReservationNumberAsync()
        {
            var prefix = "BR";
            var date = DateTime.Now.ToString("yyMMdd");
            var random = new Random();
            var sequence = random.Next(1000, 9999).ToString();

            var reservationNumber = $"{prefix}{date}{sequence}";

            // Ensure uniqueness
            while (await _context.Reservations.AnyAsync(r => r.ReservationNumber == reservationNumber))
            {
                sequence = random.Next(1000, 9999).ToString();
                reservationNumber = $"{prefix}{date}{sequence}";
            }

            return reservationNumber;
        }
    }
}
