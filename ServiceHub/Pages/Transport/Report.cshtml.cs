using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ServiceHub.Data;
using ServiceHub.Models.Transport;
using System.Security.Claims;
using ClosedXML.Excel;

namespace ServiceHub.Pages.Transport
{
    [Authorize]
    public class ReportModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ReportModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<TransportRequest> Requests { get; set; } = new();
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

            Requests = await _context.TransportRequests
                .Include(r => r.Car)
                .Where(r => r.UserId == user.Id
                    && r.TripDateTime.Date >= startDate.Date
                    && r.TripDateTime.Date <= endDate.Date)
                .OrderBy(r => r.TripDateTime)
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostDownloadAsync(DateTime startDate, DateTime endDate)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value ?? User.Identity?.Name;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
            if (user == null) return RedirectToPage("/Account/Login");

            var requests = await _context.TransportRequests
                .Include(r => r.Car)
                .Where(r => r.UserId == user.Id
                    && r.TripDateTime.Date >= startDate.Date
                    && r.TripDateTime.Date <= endDate.Date)
                .OrderBy(r => r.TripDateTime)
                .ToListAsync();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Транспортные заявки");

                // === Шапка отчёта ===
                worksheet.Cell(1, 1).Value = "ServiceHub — Корпоративный портал";
                worksheet.Cell(1, 1).Style.Font.FontSize = 14;
                worksheet.Cell(1, 1).Style.Font.Bold = true;

                worksheet.Cell(2, 1).Value = "Отчёт по транспортным заявкам";
                worksheet.Cell(2, 1).Style.Font.FontSize = 12;
                worksheet.Cell(2, 1).Style.Font.Bold = true;

                worksheet.Cell(3, 1).Value = $"Период: с {startDate:dd.MM.yyyy} по {endDate:dd.MM.yyyy}";
                worksheet.Cell(4, 1).Value = $"Пользователь: {user.LastName} {user.FirstName}";
                worksheet.Cell(5, 1).Value = $"Сформирован: {DateTime.Now:dd.MM.yyyy HH:mm}";


                // === Таблица данных ===
                int headerRow = 6;
                var tableHeaders = new[] { "№", "ID", "Дата и время", "Откуда", "Куда",
                    "Пассажиров", "Автомобиль", "Госномер", "Цель", "Статус" };

                // Стиль шапки таблицы
                var tableHeaderRange = worksheet.Range(headerRow, 1, headerRow, tableHeaders.Length);
                tableHeaderRange.Style.Font.Bold = true;
                tableHeaderRange.Style.Font.FontColor = XLColor.White;
                tableHeaderRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#0d6efd");

                for (int i = 0; i < tableHeaders.Length; i++)
                {
                    worksheet.Cell(headerRow, i + 1).Value = tableHeaders[i];
                }

                // Данные
                int row = headerRow + 1;
                for (int i = 0; i < requests.Count; i++)
                {
                    var req = requests[i];
                    var car = req.Car;

                    worksheet.Cell(row, 1).Value = i + 1;
                    worksheet.Cell(row, 2).Value = req.Id;
                    worksheet.Cell(row, 3).Value = req.TripDateTime.ToString("dd.MM.yyyy HH:mm");
                    worksheet.Cell(row, 4).Value = req.StartPoint;
                    worksheet.Cell(row, 5).Value = req.EndPoint;
                    worksheet.Cell(row, 6).Value = req.PassengerCount.ToString();
                    worksheet.Cell(row, 7).Value = car != null ? $"{car.Brand} {car.Model}" : "—";
                    worksheet.Cell(row, 8).Value = car?.LicensePlate ?? "—";
                    worksheet.Cell(row, 9).Value = req.Purpose;
                    worksheet.Cell(row, 10).Value = req.Status;

                    var statusCell = worksheet.Cell(row, 10);
                    string status = req.Status;
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
                    else if (status == "Выполнена")
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
                var dataRange = worksheet.Range(headerRow, 1, row - 1, tableHeaders.Length);
                dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                // Автоширина
                worksheet.Columns().AdjustToContents();

                // === Итоги ===
                row++; // пустая строка
                worksheet.Cell(row, 1).Value = $"Итого заявок: {requests.Count}";
                worksheet.Cell(row, 1).Style.Font.Bold = true;

                row++;
                int confirmed = requests.Count(r => r.Status == "Подтверждена");
                int completed = requests.Count(r => r.Status == "Выполнена");
                int rejected = requests.Count(r => r.Status == "Отклонена");
                int pending = requests.Count(r => r.Status == "На согласовании");
                worksheet.Cell(row, 1).Value = $"Подтверждено: {confirmed} | Выполнено: {completed} | Отклонено: {rejected} | На согласовании: {pending}";

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    string fileName = $"TransportReport_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}.xlsx";
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
        }
    }
}