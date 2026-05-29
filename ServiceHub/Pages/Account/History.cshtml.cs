using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ServiceHub.Data;
using ServiceHub.Models;
using ServiceHub.Models.Rooms;
using System.Security.Claims;

namespace ServiceHub.Pages.Account
{
    [Authorize]
    public class HistoryModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public HistoryModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<ServiceRequest> ServiceRequests { get; set; } = new();

        public void OnGet()
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value ?? User.Identity?.Name;
            if (string.IsNullOrEmpty(userEmail)) return;

            var user = _context.Users.FirstOrDefault(u => u.Email == userEmail);
            if (user == null) return;

            var userId = user.Id;

            var serviceRequests = _context.ServiceRequests
                .Where(sr => sr.UserId == userId)
                .OrderByDescending(sr => sr.CreatedAt)
                .ToList();

            var transportRequests = _context.TransportRequests
                .Include(tr => tr.Car)
                .Where(tr => tr.UserId == userId)
                .OrderByDescending(tr => tr.CreatedAt)
                .ToList();

            var roomRequests = _context.RoomRequests
                .Include(r => r.Room)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.CreatedAt)
                .ToList();

            var allRequests = new List<ServiceRequest>();
            allRequests.AddRange(serviceRequests);

            foreach (var tr in transportRequests)
            {
                var carInfo = tr.Car != null ? $"{tr.Car.Brand} {tr.Car.Model}" : "не указан";
                var description = $"{tr.StartPoint} → {tr.EndPoint}, {tr.PassengerCount} чел., авто: {carInfo}. Цель: {tr.Purpose}";
                allRequests.Add(new ServiceRequest
                {
                    Id = tr.Id,
                    ServiceType = "Транспорт",
                    Title = $"Служебная поездка {tr.TripDateTime:dd.MM HH:mm}",
                    Description = description,
                    Status = tr.Status,
                    CreatedAt = tr.CreatedAt,
                    User = tr.User,
                    UserId = tr.UserId
                });
            }

            foreach (var rr in roomRequests)
            {
                var roomName = rr.Room?.Name ?? "Не указано";
                var description = $"Помещение: {roomName}, {rr.ParticipantsCount} уч., {rr.StartTime:dd.MM.yyyy HH:mm}–{rr.EndTime:HH:mm}";
                allRequests.Add(new ServiceRequest
                {
                    Id = rr.Id,
                    ServiceType = "Помещения",
                    Title = rr.Topic,
                    Description = description,
                    Status = rr.Status,
                    CreatedAt = rr.CreatedAt,
                    User = rr.User,
                    UserId = rr.UserId
                });
            }

            ServiceRequests = allRequests.OrderByDescending(r => r.CreatedAt).ToList();
        }
    }
}