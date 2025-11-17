using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServiceHub.Migrations
{
    public partial class SeedData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Добавление начальных пользователей
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Email", "PasswordHash", "FirstName", "LastName", "Department", "CreatedAt", "IsActive" },
                values: new object[] {
                    "test@example.com",
                    "123456",
                    "Иван",
                    "Иванов",
                    "IT",
                    new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                    true
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Email", "PasswordHash", "FirstName", "LastName", "Department", "CreatedAt", "IsActive" },
                values: new object[] {
                    "admin@company.com",
                    "admin123",
                    "Администратор",
                    "Системы",
                    "Администрация",
                    new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                    true
                });

            // Добавление начальных заявок
            migrationBuilder.InsertData(
                table: "ServiceRequests",
                columns: new[] { "ServiceType", "Title", "Description", "Status", "UserId", "CreatedAt", "UpdatedAt" },
                values: new object[] {
                    "Транспорт",
                    "Заказ служебного автомобиля",
                    "Необходим автомобиль для поездки на встречу с клиентом",
                    "Ожидание",
                    1,
                    new DateTime(2024, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc),
                    null
                });

            migrationBuilder.InsertData(
                table: "ServiceRequests",
                columns: new[] { "ServiceType", "Title", "Description", "Status", "UserId", "CreatedAt", "UpdatedAt" },
                values: new object[] {
                    "Поддержка",
                    "Проблема с принтером",
                    "Не печатает сетевой принтер в кабинете 301",
                    "В работе",
                    1,
                    new DateTime(2024, 1, 16, 14, 30, 0, 0, DateTimeKind.Utc),
                    new DateTime(2024, 1, 16, 15, 0, 0, 0, DateTimeKind.Utc)
                });

            migrationBuilder.InsertData(
                table: "ServiceRequests",
                columns: new[] { "ServiceType", "Title", "Description", "Status", "UserId", "CreatedAt", "UpdatedAt" },
                values: new object[] {
                    "Помещения",
                    "Бронирование переговорной",
                    "Необходима переговорная №5 для совещания с отделом продаж",
                    "Завершено",
                    2,
                    new DateTime(2024, 1, 10, 9, 0, 0, 0, DateTimeKind.Utc),
                    new DateTime(2024, 1, 10, 12, 0, 0, 0, DateTimeKind.Utc)
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Удаление добавленных заявок
            migrationBuilder.Sql("DELETE FROM ServiceRequests WHERE Id IN (1, 2, 3)");

            // Удаление добавленных пользователей
            migrationBuilder.Sql("DELETE FROM Users WHERE Id IN (1, 2)");
        }
    }
}