using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ServiceHub.Data;
using ServiceHub.Models;

namespace ServiceHub.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class ServiceRequestsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ServiceRequestsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<ServiceRequest> ServiceRequests { get; set; } = new();

        public void OnGet()
        {
            ServiceRequests = _context.ServiceRequests
                .Include(sr => sr.User)
                .OrderByDescending(sr => sr.CreatedAt)
                .ToList();
        }

        public IActionResult OnPostDelete(int id)
        {
            var request = _context.ServiceRequests.Find(id);
            if (request != null)
            {
                _context.ServiceRequests.Remove(request);
                _context.SaveChanges();
            }
            return RedirectToPage();
        }
    }
}