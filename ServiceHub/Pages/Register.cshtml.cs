using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ServiceHub.Data;
using ServiceHub.Models;
using ServiceHub.Models.Account;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace ServiceHub.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public RegisterModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public RegisterInputModel Input { get; set; }

        public class RegisterInputModel
        {
            [Required(ErrorMessage = "Не указан Email")]
            [EmailAddress(ErrorMessage = "Некорректный формат Email")]
            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage = "Не указан пароль")]
            [DataType(DataType.Password)]
            public string Password { get; set; } = string.Empty;

            [DataType(DataType.Password)]
            [Required(ErrorMessage = "Пожалуйста, подтвердите пароль")]
            [Compare("Password", ErrorMessage = "Пароли не совпадают")]
            public string ConfirmPassword { get; set; } = string.Empty;

            [Required(ErrorMessage = "Не указано имя")]
            public string FirstName { get; set; } = string.Empty;

            [Required(ErrorMessage = "Не указана фамилия")]
            public string LastName { get; set; } = string.Empty;

            public string Department { get; set; } = string.Empty;
        }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var existingUser = _context.Users.FirstOrDefault(u => u.Email == Input.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError(string.Empty, "Пользователь с таким email уже существует");
                return Page();
            }

            bool isFirstUser = !_context.Users.Any();
            var role = isFirstUser ? "Admin" : "User";

            var user = new User
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