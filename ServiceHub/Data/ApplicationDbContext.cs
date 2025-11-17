using Microsoft.EntityFrameworkCore;
using ServiceHub.Models;

namespace ServiceHub.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<ServiceRequest> ServiceRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Конфигурация для User
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(u => u.Email).IsUnique();
                entity.Property(u => u.Email).IsRequired().HasMaxLength(255);
                entity.Property(u => u.FirstName).HasMaxLength(100);
                entity.Property(u => u.LastName).HasMaxLength(100);
                entity.Property(u => u.Department).HasMaxLength(100);
            });

            // Конфигурация для ServiceRequest
            modelBuilder.Entity<ServiceRequest>(entity =>
            {
                entity.Property(sr => sr.ServiceType).IsRequired().HasMaxLength(50);
                entity.Property(sr => sr.Title).IsRequired().HasMaxLength(200);
                entity.Property(sr => sr.Status).IsRequired().HasMaxLength(20);

                entity.HasOne(sr => sr.User)
                      .WithMany()
                      .HasForeignKey(sr => sr.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}