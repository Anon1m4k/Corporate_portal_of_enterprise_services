using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ServiceHub.Data;
using ServiceHub.Models;
using System.Security.Claims;

namespace ServiceHub.Pages.Transport
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<TransportRequest> Requests { get; set; } = new();
        public List<string> AllStatuses { get; } = new()
        {
            "На согласовании",
            "Подтверждена",
            "Выполнена",
            "Отклонена"
        };
        public string? CurrentStatus { get; set; }

        public async Task OnGetAsync(string? status)
        {
            CurrentStatus = status;

            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value ?? User.Identity?.Name;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
            if (user == null)
            {
                // Перенаправим на страницу входа, если пользователь не найден
                RedirectToPage("/Account/Login");
                return;
            }

            var query = _context.TransportRequests
                .Where(r => r.UserId == user.Id)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(r => r.Status == status);
            }

            Requests = await query
                .OrderByDescending(r => r.TripDateTime)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostCancelAsync(int id)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value ?? User.Identity?.Name;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
            if (user == null) return NotFound();

            var request = await _context.TransportRequests
                .FirstOrDefaultAsync(r => r.Id == id && r.UserId == user.Id);

            if (request == null) return NotFound();

            // Можно отменить только если статус "На согласовании"
            if (request.Status == "На согласовании")
            {
                request.Status = "Отклонена"; // Пользователь отменяет сам
                request.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Заявка отменена.";
            }
            else
            {
                TempData["ErrorMessage"] = "Нельзя отменить заявку в текущем статусе.";
            }

            return RedirectToPage();
        }
    }
}