using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ServiceHub.Data;
using ServiceHub.Models;
using System.Security.Claims;

namespace ServiceHub.Pages.Account
{
    [Authorize]
    public class DashboardModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DashboardModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public int ActiveRequests { get; set; }
        public int CompletedRequests { get; set; }
        public int PendingRequests { get; set; }
        public int TotalRequests { get; set; }
        public List<ServiceRequest> RecentRequests { get; set; } = new();

        public async Task OnGetAsync()
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value ?? User.Identity?.Name;
            if (string.IsNullOrEmpty(userEmail))
            {
                RedirectToPage("/Account/Login");
                return;
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
            if (user == null)
            {
                RedirectToPage("/Account/Login");
                return;
            }

            var userId = user.Id;

            // Загружаем обычные заявки
            var serviceRequests = await _context.ServiceRequests
                .Where(sr => sr.UserId == userId)
                .ToListAsync();

            // Загружаем транспортные заявки
            var transportRequests = await _context.TransportRequests
                .Where(tr => tr.UserId == userId)
                .ToListAsync();

            // Подсчёт статусов
            ActiveRequests = serviceRequests.Count(sr => sr.Status == "Подтверждена") +
                             transportRequests.Count(tr => tr.Status == "Подтверждена");
            CompletedRequests = serviceRequests.Count(sr => sr.Status == "Выполнена") +
                                transportRequests.Count(tr => tr.Status == "Выполнена");
            PendingRequests = serviceRequests.Count(sr => sr.Status == "На согласовании") +
                              transportRequests.Count(tr => tr.Status == "На согласовании");
            TotalRequests = serviceRequests.Count + transportRequests.Count;

            // Формирование списка последних заявок
            var recent = new List<ServiceRequest>();
            recent.AddRange(serviceRequests);
            foreach (var tr in transportRequests)
            {
                recent.Add(new ServiceRequest
                {
                    Id = tr.Id,
                    ServiceType = "Транспорт",
                    Title = $"Транспорт: {tr.TripType} {tr.TripDateTime:dd.MM HH:mm}",
                    Status = tr.Status,
                    CreatedAt = tr.CreatedAt
                });
            }

            RecentRequests = recent.OrderByDescending(r => r.CreatedAt).Take(5).ToList();
        }
    }
}