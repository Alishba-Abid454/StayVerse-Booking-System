using Hotel_Booking_System.Models.ViewModels;

namespace Hotel_Booking_System.Services.Interfaces
{
    public interface IPropertyService
    {
        Task<IEnumerable<PropertyViewModel>> GetAllPropertiesAsync();
        Task<IEnumerable<PropertyViewModel>> GetFeaturedPropertiesAsync(int count = 6);
        Task<PropertyViewModel> GetPropertyByIdAsync(int id);
        Task<IEnumerable<PropertyViewModel>> SearchPropertiesAsync(string location, string type, int? guests);
        Task<bool> IsPropertyAvailableAsync(int propertyId, DateTime checkIn, DateTime checkOut);
        Task<IEnumerable<PropertyViewModel>> GetPropertiesByTypeAsync(string type);
        Task<PropertyViewModel> CreatePropertyAsync(PropertyViewModel model);
        Task UpdatePropertyAsync(PropertyViewModel model);
        Task DeletePropertyAsync(int id);
        Task<IEnumerable<PropertyViewModel>> GetTopRatedPropertiesAsync(int count = 10);
        Task<PropertyViewModel?> GetPropertyWithDetailsAsync(int id);
    }
}
