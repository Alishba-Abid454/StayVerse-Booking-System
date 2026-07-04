using Hotel_Booking_System.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Hotel_Booking_System.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());

            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            await context.Database.EnsureCreatedAsync();

            await EnsureRolesCreatedAsync(roleManager);

            await EnsureAdminUserCreatedAsync(userManager, roleManager);

        }

        #region Role and User Seeding

        private static async Task EnsureRolesCreatedAsync(RoleManager<IdentityRole> roleManager)
        {
            string[] roleNames = { "Admin", "User", "Manager" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }

        private static async Task<User> EnsureAdminUserCreatedAsync(
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            var adminEmail = "admin@bookingbot.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new User
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "Admin",
                    LastName = "User",
                    IsActive = true,
                    IsVerified = true,
                    EmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow,
                    Address = "123 Admin St",
                    City = "Admin City",
                    Country = "Admin Country",
                    PostalCode = "12345",
                    PreferredLanguage = "en",
                    PreferredCurrency = "USD",
                    ProfileImageUrl = "/images/default-profile.png"
                };

                var result = await userManager.CreateAsync(adminUser, "Admin@123456");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
                else
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new Exception($"Failed to create admin user: {errors}");
                }
            }

            // Ensure admin is in Admin role
            if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }

            return adminUser;
        }

        #endregion
    }
}