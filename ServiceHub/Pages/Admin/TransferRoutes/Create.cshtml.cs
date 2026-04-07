using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ServiceHub.Data;
using ServiceHub.Models.Transport;
using System.ComponentModel.DataAnnotations;

namespace ServiceHub.Pages.Admin.TransferRoutes
{
    [Authorize(Roles = "Admin")]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public int CarId { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Введите название маршрута")]
        [Display(Name = "Название маршрута")]
        public string RouteName { get; set; } = string.Empty;

        [BindProperty]
        [Display(Name = "Описание")]
        public string? Description { get; set; }

        [BindProperty]
        public List<StopInput> Stops { get; set; } = new();

        public string CarName { get; set; } = string.Empty;

        public class StopInput
        {
            [Required(ErrorMessage = "Укажите адрес")]
            public string Address { get; set; } = string.Empty;

            [Required(ErrorMessage = "Укажите время")]
            [DataType(DataType.Time)]
            public TimeSpan ArrivalTime { get; set; }

            public int Order { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(int carId)
        {
            var car = await _context.Cars.FindAsync(carId);
            if (car == null) return NotFound();

            if (car.VehicleType != "Минивэн" && car.VehicleType != "Автобус")
            {
                TempData["ErrorMessage"] = "Маршруты можно добавлять только для автомобилей типа Минивэн или Автобус.";
                return RedirectToPage("/Admin/Cars/Index");
            }

            CarId = carId;
            CarName = $"{car.Brand} {car.Model}";
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Валидация остановок
            if (Stops == null || Stops.Count == 0)
            {
                ModelState.AddModelError(string.Empty, "Добавьте хотя бы одну остановку");
            }
            else
            {
                for (int i = 0; i < Stops.Count; i++)
                {
                    if (string.IsNullOrWhiteSpace(Stops[i].Address))
                    {
                        ModelState.AddModelError($"Stops[{i}].Address", "Адрес обязателен");
                    }
                }
            }

            if (!ModelState.IsValid)
            {
                var car = await _context.Cars.FindAsync(CarId);
                CarName = car != null ? $"{car.Brand} {car.Model}" : "";
                return Page();
            }

            // Создаём маршрут
            var route = new TransferRoute
            {
                Name = RouteName,
                Description = Description,
                CarId = CarId,
                IsActive = true,
                Stops = Stops.Select(s => new TransferStop
                {
                    Order = s.Order,
                    Address = s.Address,
                    ArrivalTime = s.ArrivalTime
                }).ToList()
            };

            _context.TransferRoutes.Add(route);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Маршрут успешно добавлен.";
            return RedirectToPage("Index", new { carId = CarId });
        }
    }
}