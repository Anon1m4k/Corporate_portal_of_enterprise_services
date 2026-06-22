using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ServiceHub.Data;
using ServiceHub.Models;
using ServiceHub.Models.Account;
using System.Security.Claims;

namespace ServiceHub.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public RegisterModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Models.Account.RegisterModel Input { get; set; }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            // Проверяем наличие пользователя с таким email по всей базе
            var existingUser = _context.Users.FirstOrDefault(u => u.Email == Input.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError(string.Empty, "Пользователь с таким email уже существует");
                return Page();
            }

            bool isFirstUser = !_context.Users.Any();
            var role = isFirstUser ? "Admin" : "User";
            var isActive = isFirstUser; // только первый пользователь активен сразу

            var user = new AuthUser
            {
                Email = Input.Email,
                FirstName = Input.FirstName,
                LastName = Input.LastName,
                Department = Input.Department,
                Role = role,
                CreatedAt = DateTime.UtcNow,
                IsActive = isActive
            };
            user.HashPassword(Input.Password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Автоматический вход только для активной учётной записи
            if (user.IsActive)
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
                return RedirectToPage("/Index");
            }
            else
            {
                TempData["RegistrationMessage"] = "Регистрация успешна. Ожидайте активации учётной записи администратором.";
                return RedirectToPage("/Account/Login");
            }
        }
    }
}