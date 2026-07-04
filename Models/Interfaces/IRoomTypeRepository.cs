namespace Hotel_Booking_System.Models.Interfaces
{
    public interface IRoomTypeRepository : IRepository<RoomType>
    {
        Task<IEnumerable<RoomType>> GetRoomTypesByPropertyAsync(int propertyId);
        Task<RoomType> GetRoomTypeWithDetailsAsync(int id);
        Task<IEnumerable<RoomType>> GetAvailableRoomTypesAsync(int propertyId, DateTime checkIn, DateTime checkOut);
        Task<IEnumerable<RoomType>> GetRoomTypesByCapacityAsync(int minGuests, int maxGuests);
        Task<bool> IsRoomTypeAvailableAsync(int roomTypeId, DateTime checkIn, DateTime checkOut, int quantity = 1);
        Task<int> GetAvailableQuantityAsync(int roomTypeId, DateTime checkIn, DateTime checkOut);
        Task<IEnumerable<DateTime>> GetBookedDatesAsync(int roomTypeId, DateTime startDate, DateTime endDate);
        Task UpdateAvailabilityAsync(int roomTypeId, int quantity);
        Task<IEnumerable<RoomType>> GetRoomTypesByPriceRangeAsync(decimal minPrice, decimal maxPrice);
        Task<decimal> GetAveragePriceByPropertyAsync(int propertyId);
        Task<IEnumerable<RoomType>> GetMostBookedRoomTypesAsync(int propertyId, int count = 5);
    }
}
