namespace Hotel_Booking_System.Models.ViewModels
{
    public class TableViewModel
    {
        public int Id { get; set; }
        public int PropertyId { get; set; }
        public string PropertyName { get; set; }
        public string TableNumber { get; set; }
        public int Capacity { get; set; }
        public string Location { get; set; } // Indoor, Outdoor, Patio, etc.
        public bool IsAvailable { get; set; }
        public bool IsReserved { get; set; }
        public string Status { get; set; } // Available, Reserved, Occupied, Maintenance
        public string StatusColor
        {
            get
            {
                return Status switch
                {
                    "Available" => "#059669",
                    "Reserved" => "#2563eb",
                    "Occupied" => "#d97706",
                    "Maintenance" => "#dc2626",
                    _ => "#6b7280"
                };
            }
        }
        public DateTime? ReservedFrom { get; set; }
        public DateTime? ReservedTo { get; set; }
        public string ReservedBy { get; set; }
        public int? ReservationId { get; set; }
        public List<string> Features { get; set; } // Window seat, High chair, etc.
        public string ImageUrl { get; set; }
        public string Description { get; set; }
    }
}
