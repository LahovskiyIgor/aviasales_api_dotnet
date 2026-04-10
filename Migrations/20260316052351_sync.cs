using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AirlineAPI.Migrations
{
    /// <inheritdoc />
    public partial class sync : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Password",
                table: "Passengers");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Passengers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Passengers_UserId",
                table: "Passengers",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Passengers_Users_UserId",
                table: "Passengers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Passengers_Users_UserId",
                table: "Passengers");

            migrationBuilder.DropIndex(
                name: "IX_Passengers_UserId",
                table: "Passengers");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Passengers");

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Passengers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
