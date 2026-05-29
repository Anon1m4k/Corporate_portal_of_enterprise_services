using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ServiceHub.Data;
using ServiceHub.Models.Rooms;
using System.Security.Claims;

namespace ServiceHub.Pages.Rooms
{
    [Authorize]
    public class MyBookingsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public MyBookingsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<RoomRequest> Bookings { get; set; } = new();

        public async Task OnGetAsync()
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value ?? User.Identity?.Name;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
            if (user == null) return;

            Bookings = await _context.RoomRequests
                .Include(r => r.Room)
                .Where(r => r.UserId == user.Id)
                .OrderByDescending(r => r.StartTime)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostCancelAsync(int id)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value ?? User.Identity?.Name;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
            if (user == null) return NotFound();

            var booking = await _context.RoomRequests
                .FirstOrDefaultAsync(r => r.Id == id && r.UserId == user.Id);
            if (booking == null || booking.Status != "На согласовании")
                return BadRequest();

            _context.RoomRequests.Remove(booking);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Бронирование удалено.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostCompleteAsync(int id)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value ?? User.Identity?.Name;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
            if (user == null) return NotFound();

            var booking = await _context.RoomRequests
                .FirstOrDefaultAsync(r => r.Id == id && r.UserId == user.Id);
            if (booking == null || booking.Status != "Подтверждена")
                return BadRequest();

            booking.Status = "Завершена";
            booking.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Бронирование отмечено как завершённое.";
            return RedirectToPage();
        }
    }
}