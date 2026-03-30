using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServiceHub.Data;
using ServiceHub.Models;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace ServiceHub.Pages.Transport
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public TransportRequest Input { get; set; } = new();

        public List<SelectListItem> TripTypeOptions { get; } = new()
        {
            new SelectListItem("Трансфер", "Трансфер"),
            new SelectListItem("Служебная поездка", "Служебная поездка")
        };

        public List<SelectListItem> VehicleTypeOptions { get; } = new()
        {
            new SelectListItem("Легковой", "Легковой"),
            new SelectListItem("Минивэн", "Минивэн"),
            new SelectListItem("Автобус", "Автобус"),
            new SelectListItem("Грузовой", "Грузовой")
        };      

        public void OnGet()
        {
            // Устанавливаем значение по умолчанию для даты (следующий час)
            if (Input.TripDateTime == default)
            {
                Input.TripDateTime = DateTime.Now.AddHours(1);
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            // Получаем текущего пользователя
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value ?? User.Identity?.Name;
            var user = _context.Users.FirstOrDefault(u => u.Email == userEmail);
            if (user == null)
                return NotFound("Пользователь не найден.");

            var request = new TransportRequest
            {
                UserId = user.Id,
                TripType = Input.TripType,
                TripDateTime = Input.TripDateTime,
                StartPoint = Input.StartPoint,
                EndPoint = Input.EndPoint,
                PassengerCount = Input.PassengerCount,
                Purpose = Input.Purpose,
                VehicleType = Input.VehicleType,
                VehicleModel = Input.VehicleModel,
                Status = "На согласовании",  // Сразу отправляем на согласование
                CreatedAt = DateTime.UtcNow
            };

            _context.TransportRequests.Add(request);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Заявка успешно создана и отправлена на согласование.";
            return RedirectToPage("/Transport/Index");
        }
    }
}