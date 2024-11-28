using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApp_Tickets.Migrations
{
    /// <inheritdoc />
    public partial class eliminacionCascadaAfiliados : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Afiliados_AfiliadoId",
                table: "Tickets");

            migrationBuilder.AddColumn<int>(
                name: "AfiliadoId1",
                table: "Tickets",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_AfiliadoId1",
                table: "Tickets",
                column: "AfiliadoId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Afiliados_AfiliadoId",
                table: "Tickets",
                column: "AfiliadoId",
                principalTable: "Afiliados",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Afiliados_AfiliadoId1",
                table: "Tickets",
                column: "AfiliadoId1",
                principalTable: "Afiliados",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Afiliados_AfiliadoId",
                table: "Tickets");

            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Afiliados_AfiliadoId1",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_AfiliadoId1",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "AfiliadoId1",
                table: "Tickets");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Afiliados_AfiliadoId",
                table: "Tickets",
                column: "AfiliadoId",
                principalTable: "Afiliados",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
