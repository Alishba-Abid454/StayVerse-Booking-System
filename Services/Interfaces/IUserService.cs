using Hotel_Booking_System.Models;
using Hotel_Booking_System.Models.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hotel_Booking_System.Services.Interfaces
{
    public interface IUserService
    {
        Task<User> GetUserByIdAsync(string userId);
        Task<User> GetUserByEmailAsync(string email);
        Task<User> GetUserWithReservationsAsync(string userId);
        Task<User> GetUserWithReviewsAsync(string userId);
        Task<ProfileViewModel> GetUserProfileAsync(string userId);
        Task<ProfileViewModel> UpdateUserProfileAsync(ProfileViewModel model);
        Task<IEnumerable<User>> GetActiveUsersAsync();
        Task<IEnumerable<User>> GetInactiveUsersAsync();
        Task<int> GetTotalUsersAsync();
        Task<int> GetActiveUsersCountAsync();
        Task<bool> IsEmailExistsAsync(string email);
        Task<bool> IsPhoneExistsAsync(string phoneNumber);
        Task UpdateUserStatusAsync(string userId, bool isActive);
        Task<int> GetUserBookingsCountAsync(string userId);
        Task<decimal> GetUserTotalSpentAsync(string userId);
        Task<IEnumerable<User>> GetUsersWithMostBookingsAsync(int count = 10);
        Task<Dictionary<string, int>> GetUserRegistrationStatsAsync(DateTime startDate, DateTime endDate);
    }
}