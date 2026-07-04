using Hotel_Booking_System.Models.ViewModels;

namespace Hotel_Booking_System.Services.Interfaces
{
    public interface IRoomTypeService
    {
        Task<IEnumerable<RoomTypeViewModel>> GetRoomTypesByPropertyAsync(int propertyId);
        Task<RoomTypeViewModel> GetRoomTypeByIdAsync(int id);
        Task<RoomTypeViewModel> GetRoomTypeWithDetailsAsync(int id);
        Task<IEnumerable<RoomTypeViewModel>> GetAvailableRoomTypesAsync(int propertyId, DateTime checkIn, DateTime checkOut);
        Task<IEnumerable<RoomTypeViewModel>> GetRoomTypesByCapacityAsync(int minGuests, int maxGuests);
        Task<bool> IsRoomTypeAvailableAsync(int roomTypeId, DateTime checkIn, DateTime checkOut, int quantity = 1);
        Task<int> GetAvailableQuantityAsync(int roomTypeId, DateTime checkIn, DateTime checkOut);
        Task<IEnumerable<DateTime>> GetBookedDatesAsync(int roomTypeId, DateTime startDate, DateTime endDate);
        Task<RoomTypeViewModel> CreateRoomTypeAsync(RoomTypeViewModel model);
        Task UpdateRoomTypeAsync(RoomTypeViewModel model);
        Task DeleteRoomTypeAsync(int id);
        Task UpdateAvailabilityAsync(int roomTypeId, int quantity);
        Task<IEnumerable<RoomTypeViewModel>> GetRoomTypesByPriceRangeAsync(decimal minPrice, decimal maxPrice);
        Task<decimal> GetAveragePriceByPropertyAsync(int propertyId);
        Task<IEnumerable<RoomTypeViewModel>> GetMostBookedRoomTypesAsync(int propertyId, int count = 5);
    }

}
