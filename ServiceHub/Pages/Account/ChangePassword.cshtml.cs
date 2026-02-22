using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ServiceHub.Data;
using System.ComponentModel.DataAnnotations;
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
        [Required(ErrorMessage = "Введите текущий пароль")]
        [DataType(DataType.Password)]
        [Display(Name = "Текущий пароль")]
        public string CurrentPassword { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "Введите новый пароль")]
        [StringLength(25, MinimumLength = 8, ErrorMessage = "Пароль должен быть от 8 до 25 символов")]
        [DataType(DataType.Password)]
        [Display(Name = "Новый пароль")]
        public string NewPassword { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "Подтвердите новый пароль")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Пароли не совпадают")]
        [Display(Name = "Подтверждение пароля")]
        public string ConfirmPassword { get; set; } = string.Empty;

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            // Дополнительная проверка длины (хотя StringLength уже сделает)
            if (NewPassword.Length < 8)
            {
                ModelState.AddModelError("NewPassword", "Пароль должен содержать не менее 8 символов");
                return Page();
            }

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                return RedirectToPage("/Account/Login");

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return NotFound();

            if (user.Password != CurrentPassword)
            {
                ModelState.AddModelError("CurrentPassword", "Текущий пароль неверен");
                return Page();
            }

            user.Password = NewPassword;
            await _context.SaveChangesAsync();

            TempData["PasswordChangeSuccess"] = "Пароль успешно изменён";
            return RedirectToPage();
        }
    }
}