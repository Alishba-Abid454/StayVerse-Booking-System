namespace Hotel_Booking_System.Models.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetUserByEmailAsync(string email);
        Task<User> GetUserWithReservationsAsync(string userId);
        Task<User> GetUserWithReviewsAsync(string userId);
        Task<IEnumerable<User>> GetUsersByRoleAsync(string role);
        Task<IEnumerable<User>> GetActiveUsersAsync();
        Task<IEnumerable<User>> GetInactiveUsersAsync();
        Task<IEnumerable<User>> GetUsersJoinedAfterAsync(DateTime date);
        Task<int> GetTotalUsersAsync();
        Task<int> GetActiveUsersCountAsync();
        Task<Dictionary<string, int>> GetUserRegistrationStatsAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<User>> GetUsersWithMostBookingsAsync(int count = 10);
        Task<IEnumerable<User>> GetUsersWithMostReviewsAsync(int count = 10);
        Task<bool> IsEmailExistsAsync(string email);
        Task<bool> IsPhoneExistsAsync(string phoneNumber);
        Task UpdateUserStatusAsync(string userId, bool isActive);
        Task<int> GetUserBookingsCountAsync(string userId);
        Task<decimal> GetUserTotalSpentAsync(string userId);
    }
}
