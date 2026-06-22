using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ServiceHub.Data;
using ServiceHub.Models.Rooms;

namespace ServiceHub.Pages.Rooms
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Room> Rooms { get; set; } = new();
        [BindProperty(SupportsGet = true)]
        public int? MinCapacity { get; set; }
        [BindProperty(SupportsGet = true)]
        public string? Equipment { get; set; }

        public async Task OnGetAsync()
        {
            var query = _context.Rooms.Where(r => r.IsActive).AsQueryable();
            if (MinCapacity.HasValue && MinCapacity > 0)
                query = query.Where(r => r.Capacity >= MinCapacity.Value);
            if (!string.IsNullOrWhiteSpace(Equipment))
                query = query.Where(r => r.Equipment != null && r.Equipment.Contains(Equipment));

            Rooms = await query.ToListAsync();
        }
    }
}