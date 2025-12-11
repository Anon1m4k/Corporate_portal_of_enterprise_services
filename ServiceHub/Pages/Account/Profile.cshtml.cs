using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ServiceHub.Data;
using ServiceHub.Models;
using ServiceHub.Models.Account;
using System.Security.Claims;

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
        public AuthUser CurrentUser { get; set; }

        public bool IsAdmin => User.IsInRole("Admin");

        public IActionResult OnGet(int? id = null)
        {
            if (id.HasValue && id > 0)
            {
                if (!IsAdmin)
                {
                    return Forbid();
                }

                CurrentUser = _context.Users.FirstOrDefault(u => u.Id == id.Value);
                if (CurrentUser == null)
                    return NotFound();

                return Page();
            }

            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(userEmail))
            {
                userEmail = User.Identity?.Name;
            }

            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToPage("/Account/Login");
            }

            CurrentUser = _context.Users.FirstOrDefault(u => u.Email == userEmail);

            if (CurrentUser == null)
                return NotFound();

            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
                return Page();

            var user = _context.Users.FirstOrDefault(u => u.Id == CurrentUser.Id);
            if (user == null)
                return NotFound();

            if (CurrentUser.Email != user.Email)
            {
                var existingUser = _context.Users.FirstOrDefault(u => u.Email == CurrentUser.Email && u.Id != CurrentUser.Id);
                if (existingUser != null)
                {
                    ModelState.AddModelError("CurrentUser.Email", "Пользователь с таким email уже существует");
                    return Page();
                }
            }

            user.FirstName = CurrentUser.FirstName;
            user.LastName = CurrentUser.LastName;
            user.Department = CurrentUser.Department;
            user.Email = CurrentUser.Email;

            if (IsAdmin)
            {
                user.Role = CurrentUser.Role;
            }

            _context.SaveChanges();

            TempData["SuccessMessage"] = "Данные профиля обновлены";

            return RedirectToPage();
        }
    }
}