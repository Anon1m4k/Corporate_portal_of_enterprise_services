using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ServiceHub.Data;
using ServiceHub.Models;
using ServiceHub.Models.Account;

namespace ServiceHub.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class ServiceRequestModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ServiceRequestModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public ServiceRequest ServiceRequest { get; set; }

        public List<User> Users { get; set; }

        public void OnGet(int id)
        {
            Users = _context.Users.Where(u => u.IsActive).ToList();

            if (id > 0)
            {
                ServiceRequest = _context.ServiceRequests
                    .Include(sr => sr.User)
                    .FirstOrDefault(sr => sr.Id == id);
            }
            else
            {
                ServiceRequest = new ServiceRequest();
            }
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                Users = _context.Users.Where(u => u.IsActive).ToList();
                return Page();
            }

            if (ServiceRequest.Id == 0)
            {
                _context.ServiceRequests.Add(ServiceRequest);
            }
            else
            {
                _context.ServiceRequests.Update(ServiceRequest);
            }

            _context.SaveChanges();

            return RedirectToPage("/Admin/ServiceRequests");
        }
    }
}