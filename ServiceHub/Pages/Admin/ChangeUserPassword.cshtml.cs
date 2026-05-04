using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ServiceHub.Data;
using ServiceHub.Models.Account;
using System.ComponentModel.DataAnnotations;

namespace ServiceHub.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class ChangeUserPasswordModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ChangeUserPasswordModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public int UserId { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Введите новый пароль")]
        [DataType(DataType.Password)]
        [StringLength(25, MinimumLength = 8, ErrorMessage = "Пароль должен быть от 8 до 25 символов")]
        public string NewPassword { get; set; } = string.Empty;

        [BindProperty]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Подтвердите пароль")]
        [Compare("NewPassword", ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword { get; set; } = string.Empty;

        public AuthUser? User { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            UserId = user.Id;
            User = user;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                User = await _context.Users.FindAsync(UserId);
                return Page();
            }

            var user = await _context.Users.FindAsync(UserId);
            if (user == null) return NotFound();

            user.Password = NewPassword;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Пароль успешно изменён.";
            return RedirectToPage("/Admin/Users");
        }
    }
}