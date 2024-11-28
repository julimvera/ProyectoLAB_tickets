using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApp_Tickets.Migrations
{
    /// <inheritdoc />
    public partial class actualizacionEstados : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Estados",
                keyColumn: "Id",
                keyValue: 2,
                column: "Descripcion",
                value: "Resuelto");

            migrationBuilder.UpdateData(
                table: "Estados",
                keyColumn: "Id",
                keyValue: 3,
                column: "Descripcion",
                value: "Rechazado");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Estados",
                keyColumn: "Id",
                keyValue: 2,
                column: "Descripcion",
                value: "Activo");

            migrationBuilder.UpdateData(
                table: "Estados",
                keyColumn: "Id",
                keyValue: 3,
                column: "Descripcion",
                value: "Cancelado");
        }
    }
}
