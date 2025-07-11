using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MassageBookingApi.Data;
using MassageBookingApi.Models;

namespace MassageBookingApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MasseurController : ControllerBase
    {
        private readonly BookingContext _context;

        public MasseurController(BookingContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MasseurResponse>>> GetMasseurs()
        {
            var masseurs = await _context.Masseurs
                .Where(m => m.IsActive)
                .OrderBy(m => m.Name)
                .Select(m => new MasseurResponse
                {
                    Id = m.Id,
                    Name = m.Name,
                    Description = m.Description,
                    IsActive = m.IsActive,
                    CreatedAt = m.CreatedAt
                })
                .ToListAsync();

            return Ok(masseurs);
        }




    }
}