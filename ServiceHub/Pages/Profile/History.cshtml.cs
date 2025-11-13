using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ServiceHub.Pages.Profile
{
    [Authorize]
    public class HistoryModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}