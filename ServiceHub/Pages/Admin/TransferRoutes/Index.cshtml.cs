using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ServiceHub.Data;
using ServiceHub.Models.Transport;

namespace ServiceHub.Pages.Admin.TransferRoutes
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<TransferRoute> Routes { get; set; } = new();
        public string CarName { get; set; } = string.Empty;
        public int CarId { get; set; }
        public bool CanAddRoute { get; set; }

        public async Task<IActionResult> OnGetAsync(int carId)
        {
            var car = await _context.Cars.FindAsync(carId);
            if (car == null) return NotFound();

            CarId = carId;
            CarName = $"{car.Brand} {car.Model}";
            CanAddRoute = car.VehicleType == "Минивэн" || car.VehicleType == "Автобус";

            Routes = await _context.TransferRoutes
                .Include(r => r.Stops.OrderBy(s => s.Order))
                .Where(r => r.CarId == carId)
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostToggleActiveAsync(int id, bool isActive)
        {
            var route = await _context.TransferRoutes.FindAsync(id);
            if (route == null) return NotFound();

            route.IsActive = isActive;
            await _context.SaveChangesAsync();
            return RedirectToPage(new { carId = route.CarId });
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var route = await _context.TransferRoutes
                .Include(r => r.Stops)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (route == null) return NotFound();

            _context.TransferStops.RemoveRange(route.Stops);
            _context.TransferRoutes.Remove(route);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Маршрут удалён.";
            return RedirectToPage(new { carId = route.CarId });
        }
    }
}