using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ServiceHub.Data;
using ServiceHub.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceHub.Pages.Transport
{
    [Authorize]
    public class ScheduleModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ScheduleModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<TransferRoute> Routes { get; set; } = new();

        public async Task OnGetAsync()
        {
            Routes = await _context.TransferRoutes
                .Include(r => r.Car)
                .Include(r => r.Stops.OrderBy(s => s.Order))
                .Where(r => r.IsActive)
                .ToListAsync();
        }
    }
}