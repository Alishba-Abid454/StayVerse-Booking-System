namespace Hotel_Booking_System.Models.ViewModels
{
    public class PaymentViewModel
    {
        public int Id { get; set; }
        public int ReservationId { get; set; }
        public string ReservationNumber { get; set; }
        public string TransactionId { get; set; }
        public string PaymentMethod { get; set; } // CreditCard, PayPal, Stripe, etc.
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string Status { get; set; } // Pending, Completed, Failed, Refunded
        public string StatusColor
        {
            get
            {
                return Status switch
                {
                    "Pending" => "#f59e0b",
                    "Completed" => "#059669",
                    "Failed" => "#dc2626",
                    "Refunded" => "#6b7280",
                    _ => "#6b7280"
                };
            }
        }
        public string CardLastFour { get; set; }
        public string CardBrand { get; set; }
        public string BillingAddress { get; set; }
        public string BillingCity { get; set; }
        public string BillingCountry { get; set; }
        public string BillingPostalCode { get; set; }
        public DateTime PaymentDate { get; set; }
        public DateTime? RefundDate { get; set; }
        public string RefundReason { get; set; }
        public decimal? RefundAmount { get; set; }
        public string ReceiptUrl { get; set; }
        public string InvoiceNumber { get; set; }
        public bool IsSuccessful => Status == "Completed";
        public bool IsRefunded => Status == "Refunded";
    }
}
