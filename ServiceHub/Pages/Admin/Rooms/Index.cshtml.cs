using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ServiceHub.Data;
using ServiceHub.Models.Rooms;

namespace ServiceHub.Pages.Admin.Rooms
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Room> Rooms { get; set; } = new();

        public async Task OnGetAsync()
        {
            Rooms = await _context.Rooms.ToListAsync();
        }

        public async Task<IActionResult> OnPostToggleActiveAsync(int id, bool isActive)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null) return NotFound();

            room.IsActive = isActive;
            await _context.SaveChangesAsync();
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null) return NotFound();

            // Проверим, нет ли связанных бронирований
            bool hasRequests = await _context.RoomRequests.AnyAsync(r => r.RoomId == id);
            if (hasRequests)
            {
                TempData["ErrorMessage"] = "Нельзя удалить помещение, на которое есть бронирования.";
                return RedirectToPage();
            }

            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Помещение «{room.Name}» удалено.";
            return RedirectToPage();
        }
    }
}