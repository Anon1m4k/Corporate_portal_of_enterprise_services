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

            var existingUser = _context.Users.FirstOrDefault(u => u.Email == Input.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError(string.Empty, "ѕользователь с таким email уже существует");
                return Page();
            }

            bool isFirstUser = !_context.Users.Any();
            var role = isFirstUser ? "Admin" : "User";

            var user = new AuthUser
            {
                Email = Input.Email,
                Password = Input.Password,
                FirstName = Input.FirstName,
                LastName = Input.LastName,
                Department = Input.Department,
                Role = role,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            await Authenticate(user.Email, user.Role);

            return RedirectToPage("/Index");
        }

        private async Task Authenticate(string userName, string role)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userName),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, role)
            };

            var identity = new ClaimsIdentity(claims, "Cookies",
                ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("Cookies", principal);
        }
    }
}