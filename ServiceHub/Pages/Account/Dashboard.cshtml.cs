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

            var isAdmin = User.IsInRole("Admin");
            var userId = user.Id;

            // Базовые запросы
            var serviceQuery = _context.ServiceRequests.AsQueryable();
            var transportQuery = _context.TransportRequests.AsQueryable();

            if (!isAdmin)
            {
                serviceQuery = serviceQuery.Where(sr => sr.UserId == userId);
                transportQuery = transportQuery.Where(tr => tr.UserId == userId);
            }

            var serviceRequests = await serviceQuery.ToListAsync();
            var transportRequests = await transportQuery.ToListAsync();

            // Подсчёт статусов
            ActiveRequests = serviceRequests.Count(sr => sr.Status == "Подтверждена") +
                             transportRequests.Count(tr => tr.Status == "Подтверждена");
            CompletedRequests = serviceRequests.Count(sr => sr.Status == "Выполнена") +
                                transportRequests.Count(tr => tr.Status == "Выполнена");
            PendingRequests = serviceRequests.Count(sr => sr.Status == "На согласовании") +
                              transportRequests.Count(tr => tr.Status == "На согласовании");
            TotalRequests = serviceRequests.Count + transportRequests.Count;

            // Формирование списка последних заявок (с именем пользователя для админа)
            var recent = new List<ServiceRequest>();
            foreach (var sr in serviceRequests)
            {
                recent.Add(sr);
            }
            foreach (var tr in transportRequests)
            {
                var userName = tr.User != null ? $"{tr.User.FirstName} {tr.User.LastName}" : "";
                recent.Add(new ServiceRequest
                {
                    Id = tr.Id,
                    ServiceType = "Транспорт",
                    Title = $"{tr.TripType} {tr.TripDateTime:dd.MM HH:mm}",
                    Description = $"{tr.StartPoint} → {tr.EndPoint}, {tr.PassengerCount} чел., {tr.VehicleType}",
                    Status = tr.Status,
                    CreatedAt = tr.CreatedAt,
                    User = tr.User
                });
            }

            RecentRequests = recent.OrderByDescending(r => r.CreatedAt).Take(5).ToList();
        }
    }
}