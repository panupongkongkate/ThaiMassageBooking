namespace MassageBooking.Models
{
    public class Masseur
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    // Constants for the app
    public static class Constants
    {
        public static readonly List<string> TimeSlots = new()
        {
            "09:00-10:00", "10:00-11:00", "11:00-12:00", 
            "13:00-14:00", "14:00-15:00", "15:00-16:00", "16:00-17:00"
        };

        public static readonly List<string> ThaiMonths = new()
        {
            "มกราคม", "กุมภาพันธ์", "มีนาคม", "เมษายน", "พฤษภาคม", "มิถุนายน",
            "กรกฎาคม", "สิงหาคม", "กันยายน", "ตุลาคม", "พฤศจิกายน", "ธันวาคม"
        };

        public static string GetThaiMonth(int month)
        {
            return month >= 1 && month <= 12 ? ThaiMonths[month - 1] : "";
        }

        public static int GetThaiYear(int gregorianYear)
        {
            return gregorianYear + 543;
        }
    }
}