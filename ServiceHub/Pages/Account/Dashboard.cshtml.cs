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

        public void OnGet()
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(userEmail))
            {
                userEmail = User.Identity?.Name;
            }

            if (string.IsNullOrEmpty(userEmail))
            {
                RedirectToPage("/Login");
                return;
            }

            var user = _context.Users.FirstOrDefault(u => u.Email == userEmail);
            if (user == null)
            {
                RedirectToPage("/Login");
                return;
            }

            var userId = user.Id;

            var userRequests = _context.ServiceRequests
                .Where(sr => sr.UserId == userId)
                .ToList();

            ActiveRequests = userRequests.Count(sr => sr.Status == "В работе");
            CompletedRequests = userRequests.Count(sr => sr.Status == "Завершено");
            PendingRequests = userRequests.Count(sr => sr.Status == "Ожидание");
            TotalRequests = userRequests.Count;

            RecentRequests = _context.ServiceRequests
                .Where(sr => sr.UserId == userId)
                .OrderByDescending(sr => sr.CreatedAt)
                .Take(5)
                .ToList();
        }
    }
}