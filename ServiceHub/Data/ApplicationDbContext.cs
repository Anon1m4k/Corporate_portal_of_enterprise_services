using Microsoft.EntityFrameworkCore;
using ServiceHub.Models;
using ServiceHub.Models.Account;
using ServiceHub.Models.Rooms;
using ServiceHub.Models.Transport;

namespace ServiceHub.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<AuthUser> Users { get; set; }
        public DbSet<ServiceRequest> ServiceRequests { get; set; }

        // Транспортные модели
        public DbSet<TransportRequest> TransportRequests { get; set; }
        public DbSet<Car> Cars { get; set; }
        public DbSet<TransferRoute> TransferRoutes { get; set; }
        public DbSet<TransferStop> TransferStops { get; set; }

        // Модели для помещений
        public DbSet<Room> Rooms { get; set; }
        public DbSet<RoomRequest> RoomRequests { get; set; }
    }
}