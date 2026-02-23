using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ServiceHub.Data;
using ServiceHub.Models;
using System.Security.Claims;

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

        // Используем тот же тип, что и в представлении — List<ServiceRequest>
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

            // Загружаем обычные заявки
            var serviceQuery = _context.ServiceRequests
                .Include(sr => sr.User)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status) && AllStatuses.Contains(status))
                serviceQuery = serviceQuery.Where(sr => sr.Status == status);
            if (!string.IsNullOrEmpty(type) && type != "Транспорт")
                serviceQuery = serviceQuery.Where(sr => sr.ServiceType == type);

            var serviceRequests = await serviceQuery.ToListAsync();

            // Загружаем транспортные заявки
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
                transportQuery = transportQuery.Where(tr => false); // если выбран другой тип, транспорт не показываем

            var transportRequests = await transportQuery.ToListAsync();

            // Преобразуем транспортные заявки в объекты ServiceRequest для отображения
            foreach (var tr in transportRequests)
            {
                var carInfo = tr.Car != null ? $"{tr.Car.Brand} {tr.Car.Model}" : "не указан";
                var description = $"{tr.StartPoint} → {tr.EndPoint}, {tr.PassengerCount} чел., авто: {carInfo}. Цель: {tr.Purpose}";

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

            // Сортируем по дате создания
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
            TempData["SuccessMessage"] = "Статус обновлён.";

            // Возвращаемся с сохранением фильтров
            return RedirectToPage(new { status = currentStatus, type = currentType });
        }
    }
}