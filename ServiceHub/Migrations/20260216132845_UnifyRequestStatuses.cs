using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServiceHub.Migrations
{
    /// <inheritdoc />
    public partial class UnifyRequestStatuses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
        UPDATE ServiceRequests 
        SET Status = 'На согласовании' 
        WHERE Status = 'Ожидание';
    ");

            migrationBuilder.Sql(@"
        UPDATE ServiceRequests 
        SET Status = 'Подтверждена' 
        WHERE Status = 'В работе';
    ");

            migrationBuilder.Sql(@"
        UPDATE ServiceRequests 
        SET Status = 'Выполнена' 
        WHERE Status = 'Завершено';
    ");

            migrationBuilder.Sql(@"
        UPDATE ServiceRequests 
        SET Status = 'Отклонена' 
        WHERE Status = 'Отменено';
    ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
