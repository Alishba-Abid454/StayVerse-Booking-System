namespace Hotel_Booking_System.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public int ReservationId { get; set; }
        public string TransactionId { get; set; }
        public string PaymentMethod { get; set; } // CreditCard, PayPal, Stripe, etc.
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string Status { get; set; } // Pending, Completed, Failed, Refunded
        public string CardLastFour { get; set; }
        public string CardBrand { get; set; }
        public DateTime PaymentDate { get; set; }
        public DateTime? RefundDate { get; set; }
        public string RefundReason { get; set; }

        // Navigation Property
        public virtual Reservation Reservation { get; set; }
        public int? RefundAmount { get; internal set; }
    }
}
