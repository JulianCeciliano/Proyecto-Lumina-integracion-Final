using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lumina_WEB.Migrations
{
    /// <inheritdoc />
    public partial class CreateFINAL : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Recordatorios_Usuarios_IdUsuario",
                table: "Recordatorios");

            migrationBuilder.AddForeignKey(
                name: "FK_Recordatorios_Usuarios_IdUsuario",
                table: "Recordatorios",
                column: "IdUsuario",
                principalTable: "Usuarios",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Recordatorios_Usuarios_IdUsuario",
                table: "Recordatorios");

            migrationBuilder.AddForeignKey(
                name: "FK_Recordatorios_Usuarios_IdUsuario",
                table: "Recordatorios",
                column: "IdUsuario",
                principalTable: "Usuarios",
                principalColumn: "id");
        }
    }
}
