using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AirlineAPI.Migrations
{
    /// <inheritdoc />
    public partial class FixFlightSeatCascadeDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Изменяем внешний ключ FK_FlightSeats_Tickets_TicketId с SetNull на Restrict (NoAction)
            // чтобы избежать циклов каскадного удаления
            migrationBuilder.Sql(@"
                ALTER TABLE [FlightSeats] DROP CONSTRAINT [FK_FlightSeats_Tickets_TicketId];
                ALTER TABLE [FlightSeats] ADD CONSTRAINT [FK_FlightSeats_Tickets_TicketId] 
                FOREIGN KEY [TicketId] REFERENCES [Tickets]([Id]) ON DELETE NO ACTION;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Возвращаем поведение SetNull
            migrationBuilder.Sql(@"
                ALTER TABLE [FlightSeats] DROP CONSTRAINT [FK_FlightSeats_Tickets_TicketId];
                ALTER TABLE [FlightSeats] ADD CONSTRAINT [FK_FlightSeats_Tickets_TicketId] 
                FOREIGN KEY [TicketId] REFERENCES [Tickets]([Id]) ON DELETE SET NULL;
            ");
        }
    }
}
