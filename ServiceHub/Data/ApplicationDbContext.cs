using Microsoft.EntityFrameworkCore;
using ServiceHub.Models;
using ServiceHub.Models.Account;

namespace ServiceHub.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<AuthUser> Users { get; set; }
        public DbSet<ServiceRequest> ServiceRequests { get; set; }       
    }
}