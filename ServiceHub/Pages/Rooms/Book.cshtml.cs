using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ServiceHub.Data;
using ServiceHub.Models;
using ServiceHub.Models.Rooms;
using System.Security.Claims;

namespace ServiceHub.Pages.Rooms
{
    [Authorize]
    public class BookModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public BookModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public RoomRequest RoomRequest { get; set; } = new();

        public Room? Room { get; set; }

        public async Task<IActionResult> OnGetAsync(int roomId, DateTime? date, TimeSpan? start)
        {
            Room = await _context.Rooms.FindAsync(roomId);
            if (Room == null)
                return NotFound();

            RoomRequest.RoomId = roomId;
            ViewData["PresetDate"] = date?.ToString("yyyy-MM-dd") ?? DateTime.Today.ToString("yyyy-MM-dd");
            ViewData["PresetStart"] = start.HasValue ? start.Value.ToString(@"hh\:mm") : "09:00";
            ViewData["PresetEnd"] = "";
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(
    int roomId,
    DateTime bookingDate,
    TimeSpan startTime,
    TimeSpan endTime)
        {
            // Очищаем состояние модели, так как недостающие поля будем заполнять вручную
            ModelState.Clear();

            Room = await _context.Rooms.FindAsync(roomId);
            if (Room == null)
            {
                ModelState.AddModelError(string.Empty, "Помещение не найдено.");
                return Page();
            }

            RoomRequest.RoomId = roomId;

            // Полная проверка на прошедшее время (дата + время)
            DateTime requestedStart = bookingDate.Date + startTime;
            if (requestedStart < DateTime.Now)
                ModelState.AddModelError(string.Empty, "Нельзя бронировать помещение на прошедшее время.");

            if (bookingDate.Date < DateTime.Today)
                ModelState.AddModelError(string.Empty, "Дата не может быть раньше сегодняшнего дня.");
            if (bookingDate.Date > DateTime.Today.AddDays(30))
                ModelState.AddModelError(string.Empty, "Бронирование возможно не более чем на 30 дней вперёд.");
            if (endTime <= startTime)
                ModelState.AddModelError(string.Empty, "Время окончания должно быть позже времени начала.");
            if (endTime - startTime < TimeSpan.FromMinutes(30))
                ModelState.AddModelError(string.Empty, "Минимальная длительность бронирования — 30 минут.");

            if (!ModelState.IsValid)
                return Page();

            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value ?? User.Identity?.Name;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Пользователь не найден.");
                return Page();
            }

            // Заполняем обязательные поля модели
            RoomRequest.UserId = user.Id;
            RoomRequest.StartTime = bookingDate.Date + startTime;
            RoomRequest.EndTime = bookingDate.Date + endTime;

            if (RoomRequest.ParticipantsCount > Room.Capacity)
            {
                ModelState.AddModelError("RoomRequest.ParticipantsCount",
                    $"Вместимость помещения не более {Room.Capacity} человек.");
                return Page();
            }

            // Повторная валидация полной модели
            if (!TryValidateModel(RoomRequest))
                return Page();

            var conflict = await _context.RoomRequests.AnyAsync(r =>
                r.RoomId == roomId &&
                r.Status != "Отклонена" &&
                r.StartTime < RoomRequest.EndTime &&
                r.EndTime > RoomRequest.StartTime);

            if (conflict)
            {
                ModelState.AddModelError(string.Empty, "Выбранное время занято другим бронированием.");
                return Page();
            }

            RoomRequest.Status = "На согласовании";
            RoomRequest.CreatedAt = DateTime.UtcNow;
            _context.RoomRequests.Add(RoomRequest);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Бронирование создано и отправлено на согласование.";
            return RedirectToPage("/Rooms/Index");
        }
    }
}