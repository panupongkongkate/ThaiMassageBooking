using System.ComponentModel.DataAnnotations;

namespace MassageBookingApi.Models
{
    public class Booking
    {
        [Key]
        public string Id { get; set; } = string.Empty;
        
        [Required]
        public DateTime Date { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string CustomerName { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(15)]
        public string PhoneNumber { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(50)]
        public string MasseurName { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(20)]
        public string TimeSlot { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(10)]
        public string Pincode { get; set; } = string.Empty;
        
        [Required]
        public DateTime CreatedAt { get; set; }
    }

    public class BookingRequest
    {
        [Required]
        public DateTime Date { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string CustomerName { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(15)]
        public string PhoneNumber { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(50)]
        public string MasseurName { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(20)]
        public string TimeSlot { get; set; } = string.Empty;
    }

    public class BookingResponse
    {
        public string Id { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string MasseurName { get; set; } = string.Empty;
        public string TimeSlot { get; set; } = string.Empty;
        // Note: Pincode is intentionally excluded from response for security
        public DateTime CreatedAt { get; set; }
    }

    public class BookingCreateResponse
    {
        public string Id { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string MasseurName { get; set; } = string.Empty;
        public string TimeSlot { get; set; } = string.Empty;
        public string Pincode { get; set; } = string.Empty; // Only shown when booking is created
        public DateTime CreatedAt { get; set; }
    }
}