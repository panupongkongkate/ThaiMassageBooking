namespace MassageBooking.Models
{
    public class Booking
    {
        public string Id { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string MasseurName { get; set; } = string.Empty;
        public string TimeSlot { get; set; } = string.Empty;
        public string? Pincode { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; } = "pending";

        // For display purposes
        public string DateString => Date.ToString("dd/MM/yyyy");
        public string ThaiDate => Date.ToString("dd") + " " + GetThaiMonth(Date.Month) + " " + (Date.Year + 543);

        private static string GetThaiMonth(int month)
        {
            return month switch
            {
                1 => "มกราคม",
                2 => "กุมภาพันธ์",
                3 => "มีนาคม",
                4 => "เมษายน",
                5 => "พฤษภาคม",
                6 => "มิถุนายน",
                7 => "กรกฎาคม",
                8 => "สิงหาคม",
                9 => "กันยายน",
                10 => "ตุลาคม",
                11 => "พฤศจิกายน",
                12 => "ธันวาคม",
                _ => ""
            };
        }
    }

    public class BookingRequest
    {
        public DateTime Date { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string MasseurName { get; set; } = string.Empty;
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
        public string Pincode { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}