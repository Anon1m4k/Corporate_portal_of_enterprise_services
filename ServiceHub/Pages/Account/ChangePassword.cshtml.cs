using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ServiceHub.Data;
using System.Security.Claims;

namespace ServiceHub.Pages.Account
{
    [Authorize]
    public class ChangePasswordModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ChangePasswordModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Models.Account.ChangePasswordModel Input { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                return RedirectToPage("/Account/Login");

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return NotFound();

            // Проверяем старый пароль
            if (!user.VerifyPassword(Input.CurrentPassword))
            {
                ModelState.AddModelError("Input.CurrentPassword", "Текущий пароль неверен");
                return Page();
            }

            user.HashPassword(Input.NewPassword);
            await _context.SaveChangesAsync();

            TempData["PasswordChangeSuccess"] = "Пароль успешно изменён";
            return RedirectToPage();
        }
    }
}