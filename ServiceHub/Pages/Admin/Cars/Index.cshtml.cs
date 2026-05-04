using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ServiceHub.Data;
using ServiceHub.Models.Transport;

namespace ServiceHub.Pages.Admin.Cars
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Car> Cars { get; set; } = new();

        public async Task OnGetAsync()
        {
            Cars = await _context.Cars.ToListAsync();
        }

        public async Task<IActionResult> OnPostToggleAvailabilityAsync(int id, bool isAvailable)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car == null)
            {
                return NotFound();
            }

            car.IsAvailable = isAvailable;
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car == null)
            {
                return NotFound();
            }

            bool hasRequests = await _context.TransportRequests.AnyAsync(r => r.CarId == id);
            if (hasRequests)
            {
                return RedirectToPage();
            }

            _context.Cars.Remove(car);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }
    }
}