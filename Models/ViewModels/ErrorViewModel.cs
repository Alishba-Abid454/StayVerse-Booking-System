namespace Hotel_Booking_System.Models.ViewModels
{
    public class ErrorViewModel
    {
        public string RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
        public int StatusCode { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorDescription { get; set; }
        public string Path { get; set; }
        public string Method { get; set; }
        public DateTime Timestamp { get; set; }
    }
}