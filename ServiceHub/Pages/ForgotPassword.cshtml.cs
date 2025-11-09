using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ServiceHub.Pages
{
    public class ForgotPasswordModel : PageModel
    {
        public void OnGet()
        {
        }

        public IActionResult OnPost(string email)
        {
            // Заглушка
            return RedirectToPage("/Login");
        }
    }
}