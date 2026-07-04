using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace Hotel_Booking_System.Models
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName => $"{FirstName} {LastName}";
        public DateTime? DateOfBirth { get; set; }
        public string ProfileImageUrl { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
        public bool IsVerified { get; set; }
        public int LoyaltyPoints { get; set; }
        public string PreferredLanguage { get; set; } = "en";
        public string PreferredCurrency { get; set; } = "USD";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLoginAt { get; set; }

        // Navigation Properties
        public virtual ICollection<Reservation> Reservations { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
    }
}