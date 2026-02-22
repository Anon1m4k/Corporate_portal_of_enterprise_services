using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ServiceHub.Data;
using ServiceHub.Models;
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
        public TransportRequest TransportRequest { get; set; } = new();

        public List<SelectListItem> TripTypeOptions { get; } = new()
        {
            new SelectListItem("Трансфер", "Трансфер"),
            new SelectListItem("Служебная поездка", "Служебная поездка")
        };

        public List<SelectListItem> CarOptions { get; set; } = new();

        public async Task OnGetAsync()
        {
            // Загружаем доступные автомобили
            CarOptions = await _context.Cars
                .Where(c => c.IsAvailable)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = $"{c.Brand} {c.Model} ({c.VehicleType}) - {c.PassengerCapacity} мест"
                })
                .ToListAsync();

            CarOptions.Insert(0, new SelectListItem("-- Выберите автомобиль --", ""));

            // Значение по умолчанию для даты
            if (TransportRequest.TripDateTime == default)
            {
                TransportRequest.TripDateTime = DateTime.Now.AddHours(1);
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await OnGetAsync();
                return Page();
            }

            var car = await _context.Cars.FindAsync(TransportRequest.CarId);
            if (car == null)
            {
                ModelState.AddModelError("TransportRequest.CarId", "Выбранный автомобиль не существует");
                await OnGetAsync();
                return Page();
            }

            if (!car.PassengerCapacity.HasValue)
            {
                ModelState.AddModelError("TransportRequest.CarId", "У выбранного автомобиля не указана вместимость");
                await OnGetAsync();
                return Page();
            }
            if (TransportRequest.PassengerCount > car.PassengerCapacity.Value)
            {
                ModelState.AddModelError("TransportRequest.PassengerCount",
                    $"Выбранный автомобиль вмещает не более {car.PassengerCapacity.Value} пассажиров");
                await OnGetAsync();
                return Page();
            }

            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value ?? User.Identity?.Name;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
            if (user == null)
                return NotFound("Пользователь не найден.");

            TransportRequest.UserId = user.Id;
            TransportRequest.Status = "На согласовании";
            TransportRequest.CreatedAt = DateTime.UtcNow;
            // Удалены строки с присвоением VehicleType и VehicleModel

            _context.TransportRequests.Add(TransportRequest);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Заявка успешно создана и отправлена на согласование.";
            return RedirectToPage("/Transport/Index");
        }
    }
}