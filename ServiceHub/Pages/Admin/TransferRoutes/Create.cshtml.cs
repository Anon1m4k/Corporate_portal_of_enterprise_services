using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ServiceHub.Data;
using ServiceHub.Models.Transport;

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
        public TransferRoute TransferRoute { get; set; } = new TransferRoute();

        public string CarName { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync(int carId)
        {
            var car = await _context.Cars.FindAsync(carId);
            if (car == null)
                return NotFound();

            if (car.VehicleType != "Минивэн" && car.VehicleType != "Автобус")
            {
                TempData["ErrorMessage"] = "Маршруты можно добавлять только для автомобилей типа Минивэн или Автобус.";
                return RedirectToPage("/Admin/Cars/Index");
            }

            TransferRoute = new TransferRoute
            {
                CarId = carId,
                Stops = new List<TransferStop>
                {
                    new TransferStop { Order = 1 }
                }
            };

            CarName = $"{car.Brand} {car.Model}";
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.ContainsKey("TransferRoute.Car"))
                ModelState.Remove("TransferRoute.Car");

            // Преобразуем ICollection в List для индексации
            var stopsList = TransferRoute.Stops?.ToList() ?? new List<TransferStop>();

            for (int i = 0; i < stopsList.Count; i++)
            {
                var key = $"TransferRoute.Stops[{i}].TransferRouteId";
                if (ModelState.ContainsKey(key))
                    ModelState.Remove(key);
            }

            // Дополнительная проверка остановок
            if (stopsList.Count == 0)
            {
                ModelState.AddModelError("TransferRoute.Stops", "Добавьте хотя бы одну остановку");
            }
            else
            {
                for (int i = 0; i < stopsList.Count; i++)
                {
                    if (string.IsNullOrWhiteSpace(stopsList[i].Address))
                    {
                        ModelState.AddModelError($"TransferRoute.Stops[{i}].Address", "Адрес обязателен");
                    }
                }
            }

            if (!ModelState.IsValid)
            {
                // Восстанавливаем CarName для отображения
                var car = await _context.Cars.FindAsync(TransferRoute.CarId);
                CarName = car != null ? $"{car.Brand} {car.Model}" : "";
                return Page();
            }

            // Устанавливаем Order на основе позиции в списке
            for (int i = 0; i < stopsList.Count; i++)
            {
                stopsList[i].Order = i + 1;
            }

            TransferRoute.Stops = stopsList;

            _context.TransferRoutes.Add(TransferRoute);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Маршрут успешно добавлен.";
            return RedirectToPage("Index", new { carId = TransferRoute.CarId });
        }
    }
}