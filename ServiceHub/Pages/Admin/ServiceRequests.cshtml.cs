using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ServiceHub.Data;
using ServiceHub.Models;
using System.Security.Claims;
using System.Text;

namespace ServiceHub.Pages.Admin
{
    [Authorize(Roles = "Admin")]
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

            var serviceQuery = _context.ServiceRequests
                .Include(sr => sr.User)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status) && AllStatuses.Contains(status))
                serviceQuery = serviceQuery.Where(sr => sr.Status == status);
            if (!string.IsNullOrEmpty(type) && type != "Транспорт")
                serviceQuery = serviceQuery.Where(sr => sr.ServiceType == type);

            var serviceRequests = await serviceQuery.ToListAsync();

            var transportQuery = _context.TransportRequests
                .Include(tr => tr.User)
                .Include(tr => tr.Approver)
                .Include(tr => tr.Car)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status) && AllStatuses.Contains(status))
                transportQuery = transportQuery.Where(tr => tr.Status == status);
            if (!string.IsNullOrEmpty(type) && type == "Транспорт")
                transportQuery = transportQuery.Where(tr => true);
            else if (!string.IsNullOrEmpty(type))
                transportQuery = transportQuery.Where(tr => false);

            var transportRequests = await transportQuery.ToListAsync();

            foreach (var tr in transportRequests)
            {
                var carInfo = tr.Car != null ? $"{tr.Car.Brand} {tr.Car.Model}" : "Не указан";
                var description = $"{tr.StartPoint} → {tr.EndPoint}, {tr.PassengerCount} чел., Авто: {carInfo}. Цель: {tr.Purpose}";

                serviceRequests.Add(new ServiceRequest
                {
                    Id = tr.Id,
                    ServiceType = "Транспорт",
                    Title = $"Служебная поездка {tr.TripDateTime:dd.MM HH:mm}",
                    Description = description,
                    Status = tr.Status,
                    CreatedAt = tr.CreatedAt,
                    User = tr.User,
                    UserId = tr.UserId
                });
            }

            ServiceRequests = serviceRequests.OrderByDescending(r => r.CreatedAt).ToList();
        }

        public async Task<IActionResult> OnPostChangeStatusAsync(int id, string type, string newStatus, string currentStatus, string currentType)
        {
            if (type == "Транспорт")
            {
                var request = await _context.TransportRequests.FindAsync(id);
                if (request == null)
                    return NotFound();

                request.Status = newStatus;
                request.UpdatedAt = DateTime.UtcNow;

                if (newStatus == "Подтверждена" && request.ApproverId == null)
                {
                    var adminEmail = User.FindFirst(ClaimTypes.Email)?.Value ?? User.Identity?.Name;
                    var admin = await _context.Users.FirstOrDefaultAsync(u => u.Email == adminEmail);
                    if (admin != null)
                    {
                        request.ApproverId = admin.Id;
                        request.ApprovedAt = DateTime.UtcNow;
                    }
                }
            }
            else
            {
                var request = await _context.ServiceRequests.FindAsync(id);
                if (request == null)
                    return NotFound();

                request.Status = newStatus;
                request.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return RedirectToPage(new { status = currentStatus, type = currentType });
        }

        // Обработчик POST для экспорта отчёта
        public async Task<IActionResult> OnPostExportAsync(DateTime startDate, DateTime endDate, string? status)
        {
            // Собираем обычные заявки
            var serviceQuery = _context.ServiceRequests
                .Include(sr => sr.User)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status) && AllStatuses.Contains(status))
                serviceQuery = serviceQuery.Where(sr => sr.Status == status);

            var serviceRequests = await serviceQuery
                .Where(sr => sr.CreatedAt.Date >= startDate.Date && sr.CreatedAt.Date <= endDate.Date)
                .OrderBy(sr => sr.CreatedAt)
                .ToListAsync();

            // Транспортные заявки
            var transportQuery = _context.TransportRequests
                .Include(tr => tr.User)
                .Include(tr => tr.Car)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status) && AllStatuses.Contains(status))
                transportQuery = transportQuery.Where(tr => tr.Status == status);

            var transportRequests = await transportQuery
                .Where(tr => tr.CreatedAt.Date >= startDate.Date && tr.CreatedAt.Date <= endDate.Date)
                .OrderBy(tr => tr.CreatedAt)
                .ToListAsync();

            var allItems = new List<ReportItem>();

            foreach (var sr in serviceRequests)
            {
                allItems.Add(new ReportItem
                {
                    Id = sr.Id,
                    Type = sr.ServiceType,
                    UserName = sr.User != null ? $"{sr.User.FirstName} {sr.User.LastName}" : "—",
                    Title = sr.Title,
                    Details = sr.Description,
                    CreatedAt = sr.CreatedAt,
                    Status = sr.Status
                });
            }

            foreach (var tr in transportRequests)
            {
                string details = $"{tr.StartPoint} → {tr.EndPoint}, {tr.PassengerCount} чел.";
                if (tr.Car != null)
                    details += $", авто: {tr.Car.Brand} {tr.Car.Model} ({tr.Car.LicensePlate})";
                details += $". Цель: {tr.Purpose}";

                allItems.Add(new ReportItem
                {
                    Id = tr.Id,
                    Type = "Транспорт",
                    UserName = tr.User != null ? $"{tr.User.FirstName} {tr.User.LastName}" : "—",
                    Title = $"Служебная поездка {tr.TripDateTime:dd.MM HH:mm}",
                    Details = details,
                    CreatedAt = tr.CreatedAt,
                    Status = tr.Status
                });
            }

            allItems = allItems.OrderBy(i => i.CreatedAt).ToList();

            var html = new StringBuilder();
            html.AppendLine("<!DOCTYPE html>");
            html.AppendLine("<html>");
            html.AppendLine("<head>");
            html.AppendLine("<meta charset='utf-8'>");
            html.AppendLine("<title>Отчёт по заявкам</title>");
            html.AppendLine("<style>");
            html.AppendLine("body { font-family: Arial, sans-serif; margin: 20px; background-color: #f8f9fa; }");
            html.AppendLine("h2 { color: #0d6efd; }");
            html.AppendLine("table { border-collapse: collapse; width: 100%; background: white; box-shadow: 0 2px 8px rgba(0,0,0,0.1); }");
            html.AppendLine("th { background-color: #0d6efd; color: white; padding: 10px; text-align: left; }");
            html.AppendLine("td { padding: 8px 10px; border-bottom: 1px solid #ddd; }");
            html.AppendLine("tr:hover { background-color: #f1f5f9; }");
            html.AppendLine(".badge { padding: 4px 8px; border-radius: 20px; font-size: 0.9em; }");
            html.AppendLine(".bg-warning { background-color: #ffc107; color: #212529; }");
            html.AppendLine(".bg-success { background-color: #28a745; color: white; }");
            html.AppendLine(".bg-info { background-color: #17a2b8; color: white; }");
            html.AppendLine(".bg-danger { background-color: #dc3545; color: white; }");
            html.AppendLine(".bg-secondary { background-color: #6c757d; color: white; }");
            html.AppendLine("</style>");
            html.AppendLine("</head>");
            html.AppendLine("<body>");
            html.AppendLine($"<h2>Отчёт по заявкам за период {startDate:dd.MM.yyyy} - {endDate:dd.MM.yyyy}</h2>");
            if (!string.IsNullOrEmpty(status))
                html.AppendLine($"<p><strong>Фильтр по статусу:</strong> {status}</p>");

            if (allItems.Any())
            {
                html.AppendLine("<table>");
                html.AppendLine("<thead><tr><th>ID</th><th>Тип</th><th>Пользователь</th><th>Заголовок</th><th>Детали</th><th>Дата создания</th><th>Статус</th></tr></thead>");
                html.AppendLine("<tbody>");

                foreach (var item in allItems)
                {
                    string statusClass = item.Status switch
                    {
                        "На согласовании" => "bg-warning",
                        "Подтверждена" => "bg-success",
                        "Выполнена" => "bg-info",
                        "Отклонена" => "bg-danger",
                        _ => "bg-secondary"
                    };

                    html.AppendLine("<tr>");
                    html.AppendLine($"<td>#{item.Id}</td>");
                    html.AppendLine($"<td>{item.Type}</td>");
                    html.AppendLine($"<td>{item.UserName}</td>");
                    html.AppendLine($"<td>{item.Title}</td>");
                    html.AppendLine($"<td>{item.Details}</td>");
                    html.AppendLine($"<td>{item.CreatedAt:dd.MM.yyyy HH:mm}</td>");
                    html.AppendLine($"<td><span class='badge {statusClass}'>{item.Status}</span></td>");
                    html.AppendLine("</tr>");
                }

                html.AppendLine("</tbody>");
                html.AppendLine("</table>");
            }
            else
            {
                html.AppendLine("<p>Нет заявок за выбранный период.</p>");
            }

            html.AppendLine("</body>");
            html.AppendLine("</html>");

            byte[] bytes = Encoding.UTF8.GetBytes(html.ToString());
            string fileName = $"ServiceReport_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}.html";
            return File(bytes, "text/html", fileName);
        }

        private class ReportItem
        {
            public int Id { get; set; }
            public string Type { get; set; } = string.Empty;
            public string UserName { get; set; } = string.Empty;
            public string Title { get; set; } = string.Empty;
            public string Details { get; set; } = string.Empty;
            public DateTime CreatedAt { get; set; }
            public string Status { get; set; } = string.Empty;
        }
    }
}