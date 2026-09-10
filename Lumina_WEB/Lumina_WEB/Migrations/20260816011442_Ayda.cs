using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lumina_WEB.Migrations
{
    /// <inheritdoc />
    public partial class Ayda : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Estado",
                table: "Recordatorios",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "IdUsuario",
                table: "Recordatorios",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Recordatorios_IdUsuario",
                table: "Recordatorios",
                column: "IdUsuario");

            migrationBuilder.AddForeignKey(
                name: "FK_Recordatorios_Usuarios_IdUsuario",
                table: "Recordatorios",
                column: "IdUsuario",
                principalTable: "Usuarios",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Recordatorios_Usuarios_IdUsuario",
                table: "Recordatorios");

            migrationBuilder.DropIndex(
                name: "IX_Recordatorios_IdUsuario",
                table: "Recordatorios");

            migrationBuilder.DropColumn(
                name: "IdUsuario",
                table: "Recordatorios");

            migrationBuilder.AlterColumn<string>(
                name: "Estado",
                table: "Recordatorios",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
