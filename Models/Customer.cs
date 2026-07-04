using System.ComponentModel.DataAnnotations;

namespace Hotel_Booking_System.Models
{
    public class Customer
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

        [MaxLength(150)]
        public string? Email { get; set; }

        // If this guest later created an account, link it here
        public string? UserId { get; set; }
        public User? user { get; set; }

        public DateTime FirstSeenAt { get; set; } = DateTime.UtcNow;

        public List<Reservation> Reservations { get; set; } = new();
    }
}
