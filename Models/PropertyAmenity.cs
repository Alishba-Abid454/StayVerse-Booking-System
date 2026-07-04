namespace Hotel_Booking_System.Models
{
    public class PropertyAmenity
    {
        public int Id { get; set; }
        public int PropertyId { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public string Category { get; set; } // General, Room, Food, etc.
        public bool IsFree { get; set; }
        public bool IsPopular { get; set; }

        // Navigation Property
        public virtual Property Property { get; set; }
    }
}