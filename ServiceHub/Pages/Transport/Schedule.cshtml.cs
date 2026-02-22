using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ServiceHub.Pages.Transport
{
    [Authorize]
    public class ScheduleModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}