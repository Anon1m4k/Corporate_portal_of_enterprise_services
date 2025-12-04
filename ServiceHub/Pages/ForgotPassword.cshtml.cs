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
            return RedirectToPage("/Login");
        }
    }
}

//Пока страница не используется