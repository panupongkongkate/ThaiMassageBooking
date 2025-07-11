using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MassageBookingApi.Data;
using MassageBookingApi.Models;

namespace MassageBookingApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly BookingContext _context;

        public BookingController(BookingContext context)
        {
            _context = context;
        }


        [HttpGet("month/{year}/{month}")]
        public async Task<ActionResult<IEnumerable<BookingResponse>>> GetBookingsByMonth(int year, int month)
        {
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1);

            var bookings = await _context.Bookings
                .Where(b => b.Date >= startDate && b.Date < endDate)
                .OrderBy(b => b.Date)
                .ThenBy(b => b.TimeSlot)
                .Select(b => new BookingResponse
                {
                    Id = b.Id,
                    Date = b.Date,
                    CustomerName = b.CustomerName,
                    PhoneNumber = b.PhoneNumber,
                    MasseurName = b.MasseurName,
                    TimeSlot = b.TimeSlot,
                    CreatedAt = b.CreatedAt
                })
                .ToListAsync();

            return Ok(bookings);
        }



        [HttpPost]
        public async Task<ActionResult<BookingCreateResponse>> CreateBooking(BookingRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check for conflicts
            var existingBooking = await _context.Bookings
                .FirstOrDefaultAsync(b => 
                    b.Date.Date == request.Date.Date &&
                    b.MasseurName == request.MasseurName &&
                    b.TimeSlot == request.TimeSlot);

            if (existingBooking != null)
            {
                return Conflict(new { message = "ช่วงเวลานี้มีการจองแล้ว" });
            }

            var booking = new Booking
            {
                Id = DateTime.Now.Ticks.ToString(),
                Date = request.Date,
                CustomerName = request.CustomerName,
                PhoneNumber = request.PhoneNumber,
                MasseurName = request.MasseurName,
                TimeSlot = request.TimeSlot,
                Pincode = GeneratePincode(),
                CreatedAt = DateTime.Now
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            var response = new BookingCreateResponse
            {
                Id = booking.Id,
                Date = booking.Date,
                CustomerName = booking.CustomerName,
                PhoneNumber = booking.PhoneNumber,
                MasseurName = booking.MasseurName,
                TimeSlot = booking.TimeSlot,
                Pincode = booking.Pincode,
                CreatedAt = booking.CreatedAt
            };

            return CreatedAtAction("GetBookingsByMonth", new { year = booking.Date.Year, month = booking.Date.Month }, response);
        }

        private static string GeneratePincode()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString(); // 6-digit pincode
        }
    }
}