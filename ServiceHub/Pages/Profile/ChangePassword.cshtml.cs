using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ServiceHub.Pages.Profile
{
    [Authorize]
    public class ChangePasswordModel : PageModel
    {
        [BindProperty]
        public string CurrentPassword { get; set; } = string.Empty;

        [BindProperty]
        public string NewPassword { get; set; } = string.Empty;

        [BindProperty]
        public string ConfirmPassword { get; set; } = string.Empty;

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (NewPassword != ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "Пароли не совпадают");
                return Page();
            }

            if (NewPassword.Length < 8)
            {
                ModelState.AddModelError("NewPassword", "Пароль должен содержать не менее 8 символов");
                return Page();
            }

            TempData["SuccessMessage"] = "Пароль успешно изменен";
            return Page();
        }
    }
}