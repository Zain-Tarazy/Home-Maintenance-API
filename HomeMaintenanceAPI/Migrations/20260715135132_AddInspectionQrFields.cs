using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeMaintenanceAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddInspectionQrFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "InspectionTokenExpiresAt",
                table: "Orders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InspectionTokenHash",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InspectionTokenExpiresAt",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "InspectionTokenHash",
                table: "Orders");
        }
    }
}
