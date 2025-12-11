using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace ServiceHub.Pages.Account
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly ILogger<ForgotPasswordModel> _logger;

        public ForgotPasswordModel(ILogger<ForgotPasswordModel> logger)
        {
            _logger = logger;
        }

        // ”казываем полное им€ модели, чтобы избежать конфликта
        [BindProperty]
        public Models.Account.ForgotPasswordModel Input { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            return RedirectToPage("/Account/Login");
        }
    }
}