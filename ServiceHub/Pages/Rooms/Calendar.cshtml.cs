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

        // Список всех активных помещений для фильтра
        public List<Room> AllRooms { get; set; } = new();

        // Параметры выбранного месяца
        public int SelectedYear { get; set; }
        public int SelectedMonth { get; set; }
        public int? SelectedRoomId { get; set; }

        // Данные для календарной сетки
        public DateTime FirstDayOfMonth { get; set; }
        public int DaysInMonth { get; set; }
        public int StartDayOfWeek { get; set; } // воскресенье = 0

        // Сгруппированные бронирования по дням
        public Dictionary<int, List<RoomRequest>> BookingsByDay { get; set; } = new();

        // Навигация
        public DateTime PreviousMonth => new DateTime(SelectedYear, SelectedMonth, 1).AddMonths(-1);
        public DateTime NextMonth => new DateTime(SelectedYear, SelectedMonth, 1).AddMonths(1);

        public async Task OnGetAsync(int? year, int? month, int? roomId)
        {
            var today = DateTime.Today;
            SelectedYear = year ?? today.Year;
            SelectedMonth = month ?? today.Month;
            SelectedRoomId = roomId;

            AllRooms = await _context.Rooms.Where(r => r.IsActive).ToListAsync();

            FirstDayOfMonth = new DateTime(SelectedYear, SelectedMonth, 1);
            DaysInMonth = DateTime.DaysInMonth(SelectedYear, SelectedMonth);
            int dow = (int)FirstDayOfMonth.DayOfWeek;
            StartDayOfWeek = dow == 0 ? 6 : dow - 1; 

            var startDate = FirstDayOfMonth;
            var endDate = FirstDayOfMonth.AddMonths(1).AddDays(-1);

            var query = _context.RoomRequests
                .Include(r => r.Room)
                .Where(r => r.StartTime.Date >= startDate.Date && r.StartTime.Date <= endDate.Date &&
                            (r.Status == "Подтверждена" || r.Status == "На согласовании"));

            if (SelectedRoomId.HasValue && SelectedRoomId > 0)
                query = query.Where(r => r.RoomId == SelectedRoomId.Value);

            var bookings = await query.OrderBy(r => r.StartTime).ToListAsync();

            BookingsByDay = new Dictionary<int, List<RoomRequest>>();
            for (int day = 1; day <= DaysInMonth; day++)
            {
                var date = new DateTime(SelectedYear, SelectedMonth, day);
                var dayBookings = bookings.Where(b => b.StartTime.Date == date.Date).ToList();
                if (dayBookings.Any())
                    BookingsByDay[day] = dayBookings;
            }
        }
    }
}