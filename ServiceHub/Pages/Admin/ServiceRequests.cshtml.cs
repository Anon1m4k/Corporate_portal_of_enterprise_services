using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ServiceHub.Data;
using ServiceHub.Models;
using ServiceHub.Models.Rooms;
using ServiceHub.Models.Transport;
using System.Security.Claims;

namespace ServiceHub.Pages.Admin
{
    [Authorize(Roles = "Admin,Chief")]
    public class ServiceRequestsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ServiceRequestsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<ServiceRequest> ServiceRequests { get; set; } = new();
        public List<string> AllStatuses { get; } = new()
        {
            "На согласовании",
            "Подтверждена",
            "Выполнена",
            "Отклонена"
        };
        public string? CurrentStatus { get; set; }
        public string? CurrentType { get; set; }

        public async Task OnGetAsync(string? status, string? type)
        {
            CurrentStatus = status;
            CurrentType = type;

            var allItems = new List<ServiceRequest>();

            // Обычные заявки
            List<ServiceRequest> serviceRequests;
            if (!string.IsNullOrEmpty(type) && type != "Транспорт" && type != "Помещения")
            {
                IQueryable<ServiceRequest> query = _context.ServiceRequests.Include(sr => sr.User);
                if (!string.IsNullOrEmpty(status) && AllStatuses.Contains(status))
                    query = query.Where(sr => sr.Status == status);
                query = query.Where(sr => sr.ServiceType == type);
                serviceRequests = await query.ToListAsync();
            }
            else if (string.IsNullOrEmpty(type))
            {
                IQueryable<ServiceRequest> query = _context.ServiceRequests.Include(sr => sr.User);
                if (!string.IsNullOrEmpty(status) && AllStatuses.Contains(status))
                    query = query.Where(sr => sr.Status == status);
                serviceRequests = await query.ToListAsync();
            }
            else
            {
                serviceRequests = new List<ServiceRequest>();
            }

            foreach (var sr in serviceRequests)
                allItems.Add(sr);

            // Транспортные заявки
            if (string.IsNullOrEmpty(type) || type == "Транспорт")
            {
                IQueryable<TransportRequest> query = _context.TransportRequests
                    .Include(tr => tr.User)
                    .Include(tr => tr.Car);
                if (!string.IsNullOrEmpty(status) && AllStatuses.Contains(status))
                    query = query.Where(tr => tr.Status == status);

                var transports = await query.ToListAsync();
                foreach (var tr in transports)
                {
                    var carInfo = tr.Car != null ? $"{tr.Car.Brand} {tr.Car.Model}" : "Не указан";
                    var desc = $"{tr.StartPoint} --> {tr.EndPoint}, {tr.PassengerCount} чел., авто: {carInfo}. Цель: {tr.Purpose}";
                    allItems.Add(new ServiceRequest
                    {
                        Id = tr.Id,
                        ServiceType = "Транспорт",
                        Title = $"Служебная поездка {tr.TripDateTime:dd.MM HH:mm}",
                        Description = desc,
                        Status = tr.Status,
                        CreatedAt = tr.CreatedAt,
                        User = tr.User,
                        UserId = tr.UserId
                    });
                }
            }

            // Бронирования помещений
            if (string.IsNullOrEmpty(type) || type == "Помещения")
            {
                IQueryable<RoomRequest> query = _context.RoomRequests
                    .Include(r => r.User)
                    .Include(r => r.Room);
                if (!string.IsNullOrEmpty(status) && AllStatuses.Contains(status))
                    query = query.Where(r => r.Status == status);

                var roomRequests = await query.ToListAsync();
                foreach (var rr in roomRequests)
                {
                    var roomName = rr.Room?.Name ?? "Не указано";
                    var desc = $"Помещение: {roomName}, {rr.ParticipantsCount} уч., {rr.StartTime:t}–{rr.EndTime:t}";

                    // Добавляем дополнительные пожелания, если они указаны
                    if (!string.IsNullOrWhiteSpace(rr.Description))
                    {
                        desc += $". Пожелания: {rr.Description}";
                    }

                    allItems.Add(new ServiceRequest
                    {
                        Id = rr.Id,
                        ServiceType = "Помещения",
                        Title = rr.Topic,
                        Description = desc,
                        Status = rr.Status,
                        CreatedAt = rr.CreatedAt,
                        User = rr.User,
                        UserId = rr.UserId
                    });
                }
            }

            ServiceRequests = allItems.OrderByDescending(r => r.CreatedAt).ToList();
        }

        public async Task<IActionResult> OnPostChangeStatusAsync(int id, string type, string newStatus, string currentStatus, string currentType)
        {
            if (type == "Транспорт")
            {
                var req = await _context.TransportRequests.FindAsync(id);
                if (req == null) return NotFound();
                req.Status = newStatus;
                req.UpdatedAt = DateTime.UtcNow;
                if (newStatus == "Подтверждена" && req.ApproverId == null)
                {
                    var adminEmail = User.FindFirst(ClaimTypes.Email)?.Value ?? User.Identity?.Name;
                    var admin = await _context.Users.FirstOrDefaultAsync(u => u.Email == adminEmail);
                    if (admin != null)
                    {
                        req.ApproverId = admin.Id;
                        req.ApprovedAt = DateTime.UtcNow;
                    }
                }
            }
            else if (type == "Помещения")
            {
                var req = await _context.RoomRequests.FindAsync(id);
                if (req == null) return NotFound();
                req.Status = newStatus;
                req.UpdatedAt = DateTime.UtcNow;
                if (newStatus == "Подтверждена" && req.ApproverId == null)
                {
                    var adminEmail = User.FindFirst(ClaimTypes.Email)?.Value ?? User.Identity?.Name;
                    var admin = await _context.Users.FirstOrDefaultAsync(u => u.Email == adminEmail);
                    if (admin != null)
                    {
                        req.ApproverId = admin.Id;
                        req.ApprovedAt = DateTime.UtcNow;
                    }
                }
            }
            else
            {
                var req = await _context.ServiceRequests.FindAsync(id);
                if (req == null) return NotFound();
                req.Status = newStatus;
                req.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return RedirectToPage(new { status = currentStatus, type = currentType });
        }
    }
}