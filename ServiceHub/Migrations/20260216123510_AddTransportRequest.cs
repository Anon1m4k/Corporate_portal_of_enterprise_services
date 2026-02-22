using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServiceHub.Migrations
{
    /// <inheritdoc />
    public partial class AddTransportRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TransportRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    TripType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TripDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartPoint = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EndPoint = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PassengerCount = table.Column<int>(type: "int", nullable: false),
                    Purpose = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VehicleType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VehicleModel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApproverId = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransportRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransportRequests_Users_ApproverId",
                        column: x => x.ApproverId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TransportRequests_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TransportRequests_ApproverId",
                table: "TransportRequests",
                column: "ApproverId");

            migrationBuilder.CreateIndex(
                name: "IX_TransportRequests_UserId",
                table: "TransportRequests",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TransportRequests");
        }
    }
}
