using Hotel_Booking_System.Data;
using Hotel_Booking_System.Models.Interfaces;
using Hotel_Booking_System.Models.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Hotel_Booking_System.Services
{
    public static class ServiceExtensions
    {
        public static void AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        }

        public static void AddRepositories(this IServiceCollection services)
        {
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IPropertyRepository, PropertyRepository>();
            services.AddScoped<IReservationRepository, ReservationRepository>();
            services.AddScoped<IRoomTypeRepository, RoomTypeRepository>();
            services.AddScoped<IReviewRepository, ReviewRepository>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
        }

        public static void AddServices(this IServiceCollection services)
        {
            // Add business logic services here
            // services.AddScoped<IPropertyService, PropertyService>();
            // services.AddScoped<IReservationService, ReservationService>();
        }
    }
}
