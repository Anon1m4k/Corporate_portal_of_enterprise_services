using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ServiceHub.Data;
using ServiceHub.Models.Account;
using System.Security.Claims;

namespace ServiceHub.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class UsersModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public UsersModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<AuthUser> Users { get; set; } = new();

        public async Task OnGetAsync()
        {
            Users = await _context.Users.ToListAsync();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Запрещаем удалять самого себя
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            if (user.Id == currentUserId)
            {
                TempData["ErrorMessage"] = "Нельзя удалить свою учётную запись.";
                return RedirectToPage();
            }

            // Проверяем наличие связанных заявок
            bool hasServiceRequests = await _context.ServiceRequests.AnyAsync(r => r.UserId == id);
            bool hasTransportRequests = await _context.TransportRequests.AnyAsync(r => r.UserId == id);

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Пользователь {user.FirstName} {user.LastName} удалён.";
            return RedirectToPage();
        }
    }
}