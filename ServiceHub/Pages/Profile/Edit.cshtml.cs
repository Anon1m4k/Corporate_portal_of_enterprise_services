using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ServiceHub.Pages.Profile
{
    [Authorize]
    public class EditModel : PageModel
    {
        [BindProperty]
        public string FirstName { get; set; } = "Иван";

        [BindProperty]
        public string LastName { get; set; } = "Петров";

        [BindProperty]
        public string Email { get; set; } = "test@example.com";

        [BindProperty]
        public string Phone { get; set; } = "+7 (999) 123-45-67";

        [BindProperty]
        public string Department { get; set; } = "IT";

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            TempData["SuccessMessage"] = "Данные успешно обновлены";
            return Page();
        }
    }
}