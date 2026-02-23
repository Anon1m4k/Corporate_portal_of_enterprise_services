using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ServiceHub.Data;
using ServiceHub.Models.Account;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace ServiceHub.Pages.Account
{
    [Authorize]
    public class ProfileModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ProfileModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public AuthUser CurrentUser { get; set; } = new();

        public bool IsAdmin => User.IsInRole("Admin");

        public async Task<IActionResult> OnGetAsync(int? id = null)
        {
            AuthUser? user = null;

            if (id.HasValue && id > 0)
            {
                if (!IsAdmin) return Forbid();
                user = await _context.Users.FindAsync(id.Value);
                if (user == null) return NotFound();
            }
            else
            {
                var userEmail = User.FindFirst(ClaimTypes.Email)?.Value ?? User.Identity?.Name;
                if (string.IsNullOrEmpty(userEmail)) return RedirectToPage("/Account/Login");
                user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
                if (user == null) return NotFound();
            }

            CurrentUser = user;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Только администратор может изменять профили
            if (!IsAdmin) return Forbid();

            var user = await _context.Users.FindAsync(CurrentUser.Id);
            if (user == null) return NotFound();

            // Проверка уникальности email
            if (await _context.Users.AnyAsync(u => u.Email == CurrentUser.Email && u.Id != CurrentUser.Id))
            {
                ModelState.AddModelError("CurrentUser.Email", "Пользователь с таким email уже существует");
                return Page();
            }

            // Обновление полей
            user.FirstName = CurrentUser.FirstName;
            user.LastName = CurrentUser.LastName;
            user.Email = CurrentUser.Email;
            user.Department = CurrentUser.Department;
            user.Role = CurrentUser.Role; // администратор может менять роль

            await _context.SaveChangesAsync();

            // Если редактировали свой профиль – обновляем клэймы
            var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (user.Id == currentUserId)
            {
                await RefreshUserClaims(user);
            }

            TempData["SuccessMessage"] = "Профиль обновлён";
            if (user.Id != currentUserId)
                return RedirectToPage("/Admin/Users");

            return RedirectToPage();
        }

        private async Task RefreshUserClaims(AuthUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("Department", user.Department)
            };

            var claimsIdentity = new ClaimsIdentity(claims, "Cookies");
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            await HttpContext.SignInAsync("Cookies", claimsPrincipal);
        }
    }
}