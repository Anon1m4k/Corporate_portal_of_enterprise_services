using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ServiceHub.Data;
using ServiceHub.Models;
using ServiceHub.Models.Rooms;
using ServiceHub.Models.Transport;
using System.Security.Claims;
using System.Text;

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
                    var desc = $"{tr.StartPoint} → {tr.EndPoint}, {tr.PassengerCount} чел., авто: {carInfo}. Цель: {tr.Purpose}";
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

        public async Task<IActionResult> OnPostExportAsync(DateTime startDate, DateTime endDate, string? status)
        {
            var allItems = new List<ReportItem>();
            
            // Транспорт
            IQueryable<TransportRequest> transportQuery = _context.TransportRequests
                .Include(tr => tr.User).Include(tr => tr.Car);
            if (!string.IsNullOrEmpty(status) && AllStatuses.Contains(status))
                transportQuery = transportQuery.Where(tr => tr.Status == status);
            var transports = await transportQuery
                .Where(tr => tr.CreatedAt.Date >= startDate.Date && tr.CreatedAt.Date <= endDate.Date)
                .ToListAsync();
            foreach (var tr in transports)
            {
                string car = tr.Car != null ? $"{tr.Car.Brand} {tr.Car.Model} ({tr.Car.LicensePlate})" : "Нет";
                string details = $"{tr.StartPoint} → {tr.EndPoint}, {tr.PassengerCount} чел., авто: {car}. Цель: {tr.Purpose}";
                allItems.Add(new ReportItem
                {
                    Id = tr.Id,
                    Type = "Транспорт",
                    UserName = tr.User?.FirstName + " " + tr.User?.LastName,
                    Title = $"Поездка {tr.TripDateTime:dd.MM HH:mm}",
                    Details = details,
                    CreatedAt = tr.CreatedAt,
                    Status = tr.Status
                });
            }

            // Помещения
            IQueryable<RoomRequest> roomQuery = _context.RoomRequests
                .Include(r => r.User).Include(r => r.Room);
            if (!string.IsNullOrEmpty(status) && AllStatuses.Contains(status))
                roomQuery = roomQuery.Where(r => r.Status == status);
            var rooms = await roomQuery
                .Where(r => r.CreatedAt.Date >= startDate.Date && r.CreatedAt.Date <= endDate.Date)
                .ToListAsync();
            foreach (var rr in rooms)
            {
                string roomName = rr.Room?.Name ?? "Не указано";
                string details = $"Помещение: {roomName}, {rr.ParticipantsCount} уч., {rr.StartTime:dd.MM HH:mm}–{rr.EndTime:HH:mm}";
                allItems.Add(new ReportItem
                {
                    Id = rr.Id,
                    Type = "Помещения",
                    UserName = rr.User?.FirstName + " " + rr.User?.LastName,
                    Title = rr.Topic,
                    Details = details,
                    CreatedAt = rr.CreatedAt,
                    Status = rr.Status
                });
            }

            allItems = allItems.OrderBy(i => i.CreatedAt).ToList();

            var html = new StringBuilder();
            html.AppendLine("<!DOCTYPE html><html><head><meta charset='utf-8'><title>Отчёт по заявкам</title>");
            html.AppendLine("<style>body{font-family:Arial;margin:20px}table{border-collapse:collapse;width:100%}th{background:#0d6efd;color:white;padding:8px}td{padding:6px;border-bottom:1px solid #ddd}</style>");
            html.AppendLine("</head><body>");
            html.AppendLine($"<h2>Отчёт за {startDate:dd.MM.yyyy} – {endDate:dd.MM.yyyy}</h2>");
            if (!string.IsNullOrEmpty(status)) html.AppendLine($"<p>Фильтр: {status}</p>");

            html.AppendLine("<table><tr><th>ID</th><th>Тип</th><th>Пользователь</th><th>Заголовок</th><th>Детали</th><th>Дата</th><th>Статус</th></tr>");
            foreach (var item in allItems)
            {
                html.AppendLine($"<tr><td>{item.Id}</td><td>{item.Type}</td><td>{item.UserName}</td><td>{item.Title}</td><td>{item.Details}</td><td>{item.CreatedAt:dd.MM.yyyy}</td><td>{item.Status}</td></tr>");
            }
            html.AppendLine("</table></body></html>");

            return File(Encoding.UTF8.GetBytes(html.ToString()), "text/html", $"Report_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}.html");
        }

        private class ReportItem
        {
            public int Id { get; set; }
            public string Type { get; set; } = "";
            public string UserName { get; set; } = "";
            public string Title { get; set; } = "";
            public string Details { get; set; } = "";
            public DateTime CreatedAt { get; set; }
            public string Status { get; set; } = "";
        }
    }
}