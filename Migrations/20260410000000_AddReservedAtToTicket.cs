using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace AirlineAPI.Migrations
{
    public partial class AddReservedAtToTicket : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ReservedAt",
                table: "Tickets",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReservedAt",
                table: "Tickets");
        }
    }
}
