using System.ComponentModel.DataAnnotations;

namespace Hotel_Booking_System.Models
{
    public enum TableLocation
    {
        Indoor,
        Outdoor,
        Window,
        PrivateRoom
    }

    public class Table
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(20)]
        public string TableNumber { get; set; } = string.Empty;

        [Range(1, 20)]
        public int Capacity { get; set; }

        public TableLocation Location { get; set; } = TableLocation.Indoor;

        public bool IsActive { get; set; } = true;

        // ✅ Make sure this is NOT "Tabled" - it should be "Tables" in DbContext
        public List<Reservation> Reservations { get; set; } = new();
    }
}
