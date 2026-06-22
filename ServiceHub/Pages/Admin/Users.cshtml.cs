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

        public int CurrentUserId { get; set; }

        public async Task OnGetAsync()
        {
            Users = await _context.Users.ToListAsync();
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            CurrentUserId = userIdClaim != null ? int.Parse(userIdClaim) : 0;
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            if (user.Id == currentUserId)
            {
                TempData["ErrorMessage"] = "Нельзя удалить свою учётную запись.";
                return RedirectToPage();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Пользователь {user.FirstName} {user.LastName} удалён.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostToggleActiveAsync(int id, bool isActive)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            if (user.Id == currentUserId && !isActive)
            {
                TempData["ErrorMessage"] = "Нельзя деактивировать свою учётную запись.";
                return RedirectToPage();
            }

            user.IsActive = isActive;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Пользователь {user.FirstName} {user.LastName} {(isActive ? "активирован" : "деактивирован")}.";
            return RedirectToPage();
        }
    }
}