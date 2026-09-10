using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lumina_WEB.Migrations
{
    /// <inheritdoc />
    public partial class olaaa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EntradasDiario_Usuarios_IdUsuario",
                table: "EntradasDiario");

            migrationBuilder.AddForeignKey(
                name: "FK_EntradasDiario_Usuarios_IdUsuario",
                table: "EntradasDiario",
                column: "IdUsuario",
                principalTable: "Usuarios",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EntradasDiario_Usuarios_IdUsuario",
                table: "EntradasDiario");

            migrationBuilder.AddForeignKey(
                name: "FK_EntradasDiario_Usuarios_IdUsuario",
                table: "EntradasDiario",
                column: "IdUsuario",
                principalTable: "Usuarios",
                principalColumn: "id");
        }
    }
}
