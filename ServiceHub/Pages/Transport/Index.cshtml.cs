using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ServiceHub.Data;
using ServiceHub.Models.Transport;
using System.Security.Claims;
using System.Text;

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
                RedirectToPage("/Account/Login");
                return;
            }

            var query = _context.TransportRequests
                .Include(r => r.Car)
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

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value ?? User.Identity?.Name;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
            if (user == null) return NotFound();

            var request = await _context.TransportRequests
                .FirstOrDefaultAsync(r => r.Id == id && r.UserId == user.Id);
            if (request == null) return NotFound();

            if (request.Status == "На согласовании")
            {
                _context.TransportRequests.Remove(request);
                await _context.SaveChangesAsync();
                TempData["TransportSuccess"] = "Заявка удалена.";
            }
            else
            {
                TempData["ErrorMessage"] = "Нельзя удалить заявку в текущем статусе.";
            }

            return RedirectToPage(new { status = CurrentStatus });
        }

        public async Task<IActionResult> OnPostCompleteAsync(int id)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value ?? User.Identity?.Name;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
            if (user == null) return NotFound();

            var request = await _context.TransportRequests
                .FirstOrDefaultAsync(r => r.Id == id && r.UserId == user.Id);
            if (request == null) return NotFound();

            if (request.Status == "Подтверждена")
            {
                request.Status = "Выполнена";
                request.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                TempData["TransportSuccess"] = "Заявка отмечена как выполненная.";
            }
            else
            {
                TempData["ErrorMessage"] = "Завершить можно только подтверждённые заявки.";
            }

            return RedirectToPage(new { status = CurrentStatus });
        }

        public async Task<IActionResult> OnPostExportAsync(DateTime startDate, DateTime endDate)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value ?? User.Identity?.Name;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
            if (user == null) return NotFound();

            var requests = await _context.TransportRequests
                .Include(r => r.Car)
                .Where(r => r.UserId == user.Id && r.TripDateTime.Date >= startDate.Date && r.TripDateTime.Date <= endDate.Date)
                .OrderBy(r => r.TripDateTime)
                .ToListAsync();

            var html = new StringBuilder();
            html.AppendLine("<!DOCTYPE html>");
            html.AppendLine("<html>");
            html.AppendLine("<head>");
            html.AppendLine("<meta charset='utf-8'>");
            html.AppendLine("<title>Отчёт по транспортным заявкам</title>");
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
            html.AppendLine($"<h2>Отчёт по транспортным заявкам за период {startDate:dd.MM.yyyy} - {endDate:dd.MM.yyyy}</h2>");
            html.AppendLine($"<p>Пользователь: {user.FirstName} {user.LastName} ({user.Email})</p>");

            if (requests.Any())
            {
                html.AppendLine("<table>");
                html.AppendLine("<thead><tr><th>ID</th><th>Дата и время</th><th>Откуда</th><th>Куда</th><th>Пассажиров</th><th>Автомобиль</th><th>Цель</th><th>Статус</th></tr></thead>");
                html.AppendLine("<tbody>");

                foreach (var r in requests)
                {
                    string carInfo = r.Car != null ? $"{r.Car.Brand} {r.Car.Model} ({r.Car.LicensePlate})" : "—";
                    var statusClasses = new Dictionary<string, string>
                    {
                        ["На согласовании"] = "bg-warning",
                        ["Подтверждена"] = "bg-success",
                        ["Выполнена"] = "bg-info",
                        ["Отклонена"] = "bg-danger"
                    };
                    string statusClass = statusClasses.GetValueOrDefault(r.Status, "bg-secondary");

                    html.AppendLine("<tr>");
                    html.AppendLine($"<td>#{r.Id}</td>");
                    html.AppendLine($"<td>{r.TripDateTime:dd.MM.yyyy HH:mm}</td>");
                    html.AppendLine($"<td>{r.StartPoint}</td>");
                    html.AppendLine($"<td>{r.EndPoint}</td>");
                    html.AppendLine($"<td>{r.PassengerCount}</td>");
                    html.AppendLine($"<td>{carInfo}</td>");
                    html.AppendLine($"<td>{r.Purpose}</td>");
                    html.AppendLine($"<td><span class='badge {statusClass}'>{r.Status}</span></td>");
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
            string fileName = $"TransportReport_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}.html";
            return File(bytes, "text/html", fileName);
        }
    }
}