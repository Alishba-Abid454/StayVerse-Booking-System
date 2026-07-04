using Hotel_Booking_System.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Hotel_Booking_System.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Property> Properties { get; set; }
        public DbSet<RoomType> RoomTypes { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<PropertyAmenity> PropertyAmenities { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Table> Tables { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ✅ Configure JSON storage for GalleryImages
            modelBuilder.Entity<Property>(entity =>
            {
                entity.Property(p => p.GalleryImages)
                    .HasConversion(
                        v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions)null),
                        v => System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, (System.Text.Json.JsonSerializerOptions)null)
                    );
            });

            // Decimal precision configurations
            modelBuilder.Entity<Payment>(entity =>
            {
                entity.Property(p => p.Amount)
                    .HasPrecision(18, 2);
            });

            modelBuilder.Entity<Property>(entity =>
            {
                entity.Property(p => p.BasePrice)
                    .HasPrecision(18, 2);
            });

            modelBuilder.Entity<Reservation>(entity =>
            {
                entity.Property(r => r.PricePerNight)
                    .HasPrecision(18, 2);
                entity.Property(r => r.TotalPrice)
                    .HasPrecision(18, 2);
            });

            modelBuilder.Entity<RoomType>(entity =>
            {
                entity.Property(r => r.PricePerNight)
                    .HasPrecision(18, 2);
                entity.Property(r => r.WeekendPrice)
                    .HasPrecision(18, 2);
            });

            // User Configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(u => u.FirstName).HasMaxLength(100);
                entity.Property(u => u.LastName).HasMaxLength(100);
                entity.Property(u => u.Country).HasMaxLength(100);
                entity.Property(u => u.City).HasMaxLength(100);
                entity.Property(u => u.Address).HasMaxLength(300);
                entity.Property(u => u.PostalCode).HasMaxLength(20);
                entity.Property(u => u.ProfileImageUrl).HasMaxLength(500);
                entity.Property(u => u.PreferredLanguage).HasMaxLength(10);
                entity.Property(u => u.PreferredCurrency).HasMaxLength(10);
                entity.Property(u => u.IsActive).HasDefaultValue(true);
                entity.Property(u => u.IsVerified).HasDefaultValue(false);
                entity.Property(u => u.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.HasIndex(u => u.Email).IsUnique();
                entity.HasIndex(u => u.PhoneNumber);
                entity.HasIndex(u => u.IsActive);
            });

            // Table Configuration
            modelBuilder.Entity<Table>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.Property(t => t.TableNumber)
                    .IsRequired()
                    .HasMaxLength(20);
                entity.Property(t => t.Capacity)
                    .IsRequired();
                entity.Property(t => t.Location)
                    .HasConversion<string>();
                entity.Property(t => t.IsActive)
                    .HasDefaultValue(true);
                entity.HasIndex(t => t.TableNumber)
                    .IsUnique();
            });
        }
    }
}