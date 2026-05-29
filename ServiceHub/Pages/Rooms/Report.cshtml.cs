using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ServiceHub.Data;
using ServiceHub.Models.Rooms;
using ClosedXML.Excel;
using System.Security.Claims;

namespace ServiceHub.Pages.Rooms
{
    [Authorize]
    public class ReportModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ReportModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<RoomRequest> Bookings { get; set; } = new();
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string UserName { get; set; } = string.Empty;
        public DateTime GeneratedAt { get; set; }

        public async Task<IActionResult> OnGetAsync(DateTime startDate, DateTime endDate)
        {
            StartDate = startDate;
            EndDate = endDate;

            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value ?? User.Identity?.Name;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
            if (user == null) return RedirectToPage("/Account/Login");

            UserName = $"{user.LastName} {user.FirstName}";
            GeneratedAt = DateTime.Now;

            Bookings = await _context.RoomRequests
                .Include(r => r.Room)
                .Where(r => r.UserId == user.Id
                    && r.StartTime.Date >= startDate.Date
                    && r.StartTime.Date <= endDate.Date)
                .OrderBy(r => r.StartTime)
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostDownloadAsync(DateTime startDate, DateTime endDate)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value ?? User.Identity?.Name;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
            if (user == null) return RedirectToPage("/Account/Login");

            var bookings = await _context.RoomRequests
                .Include(r => r.Room)
                .Where(r => r.UserId == user.Id
                    && r.StartTime.Date >= startDate.Date
                    && r.StartTime.Date <= endDate.Date)
                .OrderBy(r => r.StartTime)
                .ToListAsync();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Бронирования помещений");

                // === Шапка отчёта ===
                worksheet.Cell(1, 1).Value = "ServiceHub — Корпоративный портал";
                worksheet.Cell(1, 1).Style.Font.FontSize = 14;
                worksheet.Cell(1, 1).Style.Font.Bold = true;

                worksheet.Cell(2, 1).Value = "Отчёт по бронированиям помещений";
                worksheet.Cell(2, 1).Style.Font.FontSize = 12;
                worksheet.Cell(2, 1).Style.Font.Bold = true;

                worksheet.Cell(3, 1).Value = $"Период: с {startDate:dd.MM.yyyy} по {endDate:dd.MM.yyyy}";
                worksheet.Cell(4, 1).Value = $"Пользователь: {user.LastName} {user.FirstName}";
                worksheet.Cell(5, 1).Value = $"Сформирован: {DateTime.Now:dd.MM.yyyy HH:mm}";


                // === Таблица данных ===
                int headerRow = 6;
                var headers = new[] { "№", "ID", "Помещение", "Дата", "Время", "Участников", "Тема", "Пожелания", "Статус" };
                var tableHeaderRange = worksheet.Range(headerRow, 1, headerRow, headers.Length);
                tableHeaderRange.Style.Font.Bold = true;
                tableHeaderRange.Style.Font.FontColor = XLColor.White;
                tableHeaderRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#0d6efd");

                for (int i = 0; i < headers.Length; i++)
                    worksheet.Cell(headerRow, i + 1).Value = headers[i];

                int row = headerRow + 1;
                for (int i = 0; i < bookings.Count; i++)
                {
                    var b = bookings[i];
                    worksheet.Cell(row, 1).Value = i + 1;
                    worksheet.Cell(row, 2).Value = b.Id;
                    worksheet.Cell(row, 3).Value = b.Room?.Name ?? "—";
                    worksheet.Cell(row, 4).Value = b.StartTime.ToString("dd.MM.yyyy");
                    worksheet.Cell(row, 5).Value = $"{b.StartTime:HH:mm} – {b.EndTime:HH:mm}";
                    worksheet.Cell(row, 6).Value = b.ParticipantsCount.ToString() ?? "—";
                    worksheet.Cell(row, 7).Value = b.Topic;
                    worksheet.Cell(row, 8).Value = string.IsNullOrWhiteSpace(b.Description) ? "—" : b.Description;
                    worksheet.Cell(row, 9).Value = b.Status;

                    var statusCell = worksheet.Cell(row, 9);
                    string status = b.Status;
                    if (status == "На согласовании")
                    {
                        statusCell.Style.Fill.BackgroundColor = XLColor.FromHtml("#FFC107");
                        statusCell.Style.Font.FontColor = XLColor.Black;
                    }
                    else if (status == "Подтверждена")
                    {
                        statusCell.Style.Fill.BackgroundColor = XLColor.FromHtml("#28A745");
                        statusCell.Style.Font.FontColor = XLColor.White;
                    }
                    else if (status == "Завершена")
                    {
                        statusCell.Style.Fill.BackgroundColor = XLColor.FromHtml("#17A2B8");
                        statusCell.Style.Font.FontColor = XLColor.White;
                    }
                    else if (status == "Отклонена")
                    {
                        statusCell.Style.Fill.BackgroundColor = XLColor.FromHtml("#DC3545");
                        statusCell.Style.Font.FontColor = XLColor.White;
                    }
                    row++;
                }

                // Границы таблицы
                var dataRange = worksheet.Range(headerRow, 1, row - 1, headers.Length);
                dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                worksheet.Columns().AdjustToContents();

                // === Итоги ===
                row++;
                worksheet.Cell(row, 1).Value = $"Итого бронирований: {bookings.Count}";
                worksheet.Cell(row, 1).Style.Font.Bold = true;

                row++;
                int confirmed = bookings.Count(r => r.Status == "Подтверждена");
                int completed = bookings.Count(r => r.Status == "Завершена");
                int rejected = bookings.Count(r => r.Status == "Отклонена");
                int pending = bookings.Count(r => r.Status == "На согласовании");
                worksheet.Cell(row, 1).Value = $"Подтверждено: {confirmed} | Завершено: {completed} | Отклонено: {rejected} | На согласовании: {pending}";

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    string fileName = $"RoomBookingsReport_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}.xlsx";
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
        }
    }
}