namespace Hotel_Booking_System.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int PropertyId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string Country { get; set; }
        public double Rating { get; set; }
        public double StaffRating { get; set; }
        public double FacilitiesRating { get; set; }
        public double CleanlinessRating { get; set; }
        public double ComfortRating { get; set; }
        public double ValueForMoneyRating { get; set; }
        public double LocationRating { get; set; }
        public string Title { get; set; }
        public string Comment { get; set; }
        public string Pros { get; set; }
        public string Cons { get; set; }
        public bool IsVerified { get; set; }
        public bool IsRecommended { get; set; }
        public DateTime StayDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Navigation Property
        public virtual Property Property { get; set; }
        public int HelpfulCount { get; internal set; }
    }

}
