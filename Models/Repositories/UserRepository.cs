using Hotel_Booking_System.Data;
using Hotel_Booking_System.Models.Interfaces;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Hotel_Booking_System.Models.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository  // Use 'User' not 'IdentityUser'
    {
        public UserRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User> GetUserWithReservationsAsync(string userId)
        {
            return await _context.Users
                .Include(u => u.Reservations)
                    .ThenInclude(r => r.Property)
                .Include(u => u.Reservations)
                    .ThenInclude(r => r.RoomType)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<IEnumerable<User>> GetActiveUsersAsync()
        {
            return await _context.Users
                .Where(u => u.IsActive)  // 'IsActive' exists on your custom User class
                .OrderByDescending(u => u.CreatedAt)  // 'CreatedAt' exists on your custom User class
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetInactiveUsersAsync()
        {
            return await _context.Users
                .Where(u => !u.IsActive)
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetUsersJoinedAfterAsync(DateTime date)
        {
            return await _context.Users
                .Where(u => u.CreatedAt >= date && u.IsActive)
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();
        }

        public async Task<int> GetTotalUsersAsync()
        {
            return await _context.Users.CountAsync();
        }

        public async Task<int> GetActiveUsersCountAsync()
        {
            return await _context.Users.CountAsync(u => u.IsActive);
        }

        public async Task<Dictionary<string, int>> GetUserRegistrationStatsAsync(DateTime startDate, DateTime endDate)
        {
            var stats = new Dictionary<string, int>();

            var registrations = await _context.Users
                .Where(u => u.CreatedAt >= startDate && u.CreatedAt <= endDate)
                .GroupBy(u => u.CreatedAt.ToString("yyyy-MM-dd"))
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Date, x => x.Count);

            foreach (var reg in registrations)
            {
                stats[reg.Key] = reg.Value;
            }

            return stats;
        }

        public async Task<IEnumerable<User>> GetUsersWithMostBookingsAsync(int count = 10)
        {
            var userBookings = await _context.Reservations
                .Where(r => r.Status != "Cancelled")
                .GroupBy(r => r.UserId)
                .Select(g => new { UserId = g.Key, BookingCount = g.Count() })
                .OrderByDescending(x => x.BookingCount)
                .Take(count)
                .ToListAsync();

            var userIds = userBookings.Select(x => x.UserId).ToList();
            return await _context.Users
                .Where(u => userIds.Contains(u.Id))
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetUsersWithMostReviewsAsync(int count = 10)
        {
            var userReviews = await _context.Reviews
                .GroupBy(r => r.UserId)
                .Select(g => new { UserId = g.Key, ReviewCount = g.Count() })
                .OrderByDescending(x => x.ReviewCount)
                .Take(count)
                .ToListAsync();

            var userIds = userReviews.Select(x => x.UserId).ToList();
            return await _context.Users
                .Where(u => userIds.Contains(u.Id))
                .ToListAsync();
        }

        public async Task<bool> IsEmailExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<bool> IsPhoneExistsAsync(string phoneNumber)
        {
            return await _context.Users.AnyAsync(u => u.PhoneNumber == phoneNumber);
        }

        public async Task UpdateUserStatusAsync(string userId, bool isActive)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.IsActive = isActive;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int> GetUserBookingsCountAsync(string userId)
        {
            return await _context.Reservations
                .CountAsync(r => r.UserId == userId && r.Status != "Cancelled");
        }

        public async Task<decimal> GetUserTotalSpentAsync(string userId)
        {
            return await _context.Reservations
                .Where(r => r.UserId == userId && r.Status != "Cancelled" && r.IsPaid)
                .SumAsync(r => r.TotalPrice);
        }

        public Task<User> GetUserWithReviewsAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<User>> GetUsersByRoleAsync(string role)
        {
            throw new NotImplementedException();
        }
    }
}
