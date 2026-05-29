using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ServiceHub.Data;
using ServiceHub.Models.Transport;
using ServiceHub.Models.Rooms;
using ClosedXML.Excel;
using System.Security.Claims;

namespace ServiceHub.Pages.Admin
{
    [Authorize(Roles = "Admin,Chief")]
    public class ReportModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ReportModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<ReportItem> Items { get; set; } = new();
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? FilterStatus { get; set; }
        public string UserName { get; set; } = string.Empty;
        public DateTime GeneratedAt { get; set; }

        public async Task OnGetAsync(DateTime startDate, DateTime endDate, string? status)
        {
            StartDate = startDate;
            EndDate = endDate;
            FilterStatus = status;

            // Получаем текущего пользователя для отображения
            var currentUserEmail = User.FindFirst(ClaimTypes.Email)?.Value ?? User.Identity?.Name;
            var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == currentUserEmail);
            UserName = currentUser != null ? $"{currentUser.LastName} {currentUser.FirstName}" : "Неизвестно";
            GeneratedAt = DateTime.Now;

            // Транспортные заявки
            IQueryable<TransportRequest> transportQuery = _context.TransportRequests
                .Include(tr => tr.User)
                .Include(tr => tr.Car)
                .Where(tr => tr.CreatedAt.Date >= startDate && tr.CreatedAt.Date <= endDate);
            if (!string.IsNullOrEmpty(status))
                transportQuery = transportQuery.Where(tr => tr.Status == status);

            var transports = await transportQuery.ToListAsync();
            foreach (var tr in transports)
            {
                var car = tr.Car;
                var details = $"{tr.StartPoint} --> {tr.EndPoint}, {tr.PassengerCount} чел., авто: {(car != null ? $"{car.Brand} {car.Model} ({car.LicensePlate})" : "—")}";
                // Добавляем цель поездки
                if (!string.IsNullOrWhiteSpace(tr.Purpose))
                    details += $". Цель: {tr.Purpose}";

                Items.Add(new ReportItem
                {
                    Type = "Транспорт",
                    UserName = $"{tr.User?.LastName} {tr.User?.FirstName}",
                    Title = $"Поездка {tr.TripDateTime:dd.MM HH:mm}",
                    Details = details,
                    CreatedAt = tr.CreatedAt,
                    Status = tr.Status
                });
            }

            // Бронирования помещений
            IQueryable<RoomRequest> roomQuery = _context.RoomRequests
                .Include(r => r.User)
                .Include(r => r.Room)
                .Where(r => r.CreatedAt.Date >= startDate && r.CreatedAt.Date <= endDate);
            if (!string.IsNullOrEmpty(status))
                roomQuery = roomQuery.Where(r => r.Status == status);

            var rooms = await roomQuery.ToListAsync();
            foreach (var rr in rooms)
            {
                var details = $"Помещение: {rr.Room?.Name ?? "—"}, {rr.ParticipantsCount} уч., {rr.StartTime:dd.MM HH:mm} – {rr.EndTime:HH:mm}";
                // Добавляем дополнительные пожелания
                if (!string.IsNullOrWhiteSpace(rr.Description))
                    details += $". Пожелания: {rr.Description}";

                Items.Add(new ReportItem
                {
                    Type = "Помещения",
                    UserName = $"{rr.User?.LastName} {rr.User?.FirstName}",
                    Title = rr.Topic,
                    Details = details,
                    CreatedAt = rr.CreatedAt,
                    Status = rr.Status
                });
            }

            Items = Items.OrderBy(i => i.CreatedAt).ToList();
        }

        public async Task<IActionResult> OnPostDownloadAsync(DateTime startDate, DateTime endDate, string? status)
        {
            // Повторяем логику построения данных
            await OnGetAsync(startDate, endDate, status);
            var items = Items;

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Сводный отчёт");

                // === Шапка отчёта ===
                worksheet.Cell(1, 1).Value = "ServiceHub — Корпоративный портал";
                worksheet.Cell(1, 1).Style.Font.FontSize = 14;
                worksheet.Cell(1, 1).Style.Font.Bold = true;
                worksheet.Cell(2, 1).Value = "Сводный отчёт по заявкам";
                worksheet.Cell(2, 1).Style.Font.FontSize = 12;
                worksheet.Cell(2, 1).Style.Font.Bold = true;
                worksheet.Cell(3, 1).Value = $"Период: с {startDate:dd.MM.yyyy} по {endDate:dd.MM.yyyy}";
                worksheet.Cell(4, 1).Value = $"Пользователь: {UserName}";
                worksheet.Cell(5, 1).Value = $"Сформирован: {DateTime.Now:dd.MM.yyyy HH:mm}";

                if (!string.IsNullOrEmpty(status))
                    worksheet.Cell(5, 1).Value = $"Фильтр по статусу: {status}";

                int headerRow = string.IsNullOrEmpty(status) ? 6 : 7;
                var headers = new[] { "№", "Тип", "Автор", "Заголовок", "Детали", "Дата создания", "Статус" };
                var tableHeaderRange = worksheet.Range(headerRow, 1, headerRow, headers.Length);
                tableHeaderRange.Style.Font.Bold = true;
                tableHeaderRange.Style.Font.FontColor = XLColor.White;
                tableHeaderRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#0d6efd");

                for (int i = 0; i < headers.Length; i++)
                    worksheet.Cell(headerRow, i + 1).Value = headers[i];

                int row = headerRow + 1;
                for (int i = 0; i < items.Count; i++)
                {
                    var item = items[i];
                    worksheet.Cell(row, 1).Value = i + 1;
                    worksheet.Cell(row, 2).Value = item.Type;
                    worksheet.Cell(row, 3).Value = item.UserName;
                    worksheet.Cell(row, 4).Value = item.Title;
                    worksheet.Cell(row, 5).Value = item.Details;
                    worksheet.Cell(row, 6).Value = item.CreatedAt.ToString("dd.MM.yyyy");
                    worksheet.Cell(row, 7).Value = item.Status;

                    var statusCell = worksheet.Cell(row, 7);
                    string itemStatus = item.Status;
                    if (itemStatus == "На согласовании")
                    {
                        statusCell.Style.Fill.BackgroundColor = XLColor.FromHtml("#FFC107");
                        statusCell.Style.Font.FontColor = XLColor.Black;
                    }
                    else if (itemStatus == "Подтверждена")
                    {
                        statusCell.Style.Fill.BackgroundColor = XLColor.FromHtml("#28A745");
                        statusCell.Style.Font.FontColor = XLColor.White;
                    }
                    else if (itemStatus == "Завершена")
                    {
                        statusCell.Style.Fill.BackgroundColor = XLColor.FromHtml("#17A2B8");
                        statusCell.Style.Font.FontColor = XLColor.White;
                    }
                    else if (itemStatus == "Отклонена")
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
                worksheet.Cell(row, 1).Value = $"Итого заявок: {items.Count}";
                worksheet.Cell(row, 1).Style.Font.Bold = true;

                row++;
                int confirmed = items.Count(i => i.Status == "Подтверждена");
                int completed = items.Count(i => i.Status == "Завершена");
                int rejected = items.Count(i => i.Status == "Отклонена");
                int pending = items.Count(i => i.Status == "На согласовании");
                worksheet.Cell(row, 1).Value = $"Подтверждено: {confirmed} | Завершено: {completed} | Отклонено: {rejected} | На согласовании: {pending}";

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    string fileName = $"CombinedReport_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}.xlsx";
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
        }

        public class ReportItem
        {
            public string Type { get; set; } = "";
            public string UserName { get; set; } = "";
            public string Title { get; set; } = "";
            public string Details { get; set; } = "";
            public DateTime CreatedAt { get; set; }
            public string Status { get; set; } = "";
        }
    }
}