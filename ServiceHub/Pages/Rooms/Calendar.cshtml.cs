using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ServiceHub.Data;
using ServiceHub.Models.Rooms;

namespace ServiceHub.Pages.Rooms
{
    public class CalendarModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CalendarModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Room> AllRooms { get; set; } = new();
        public int? SelectedRoomId { get; set; }
        public DateTime SelectedDate { get; set; } = DateTime.Today;
        public List<RoomRequest> Bookings { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int? roomId, DateTime? selectedDate)
        {
            AllRooms = await _context.Rooms.Where(r => r.IsActive).ToListAsync();
            SelectedRoomId = roomId;
            SelectedDate = selectedDate?.Date ?? DateTime.Today;

            var query = _context.RoomRequests
                .Include(r => r.Room)
                .Where(r => r.StartTime.Date == SelectedDate.Date &&
                            (r.Status == "Подтверждена" || r.Status == "На согласовании"));

            if (roomId.HasValue && roomId > 0)
                query = query.Where(r => r.RoomId == roomId.Value);

            Bookings = await query.OrderBy(r => r.StartTime).ToListAsync();
            return Page();
        }
    }
}