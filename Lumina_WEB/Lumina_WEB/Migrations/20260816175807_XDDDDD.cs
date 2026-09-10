using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lumina_WEB.Migrations
{
    /// <inheritdoc />
    public partial class XDDDDD : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Alertas_IdTipoAlerta",
                table: "Alertas",
                column: "IdTipoAlerta");

            migrationBuilder.AddForeignKey(
                name: "FK_Alertas_TiposAlerta_IdTipoAlerta",
                table: "Alertas",
                column: "IdTipoAlerta",
                principalTable: "TiposAlerta",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Alertas_TiposAlerta_IdTipoAlerta",
                table: "Alertas");

            migrationBuilder.DropIndex(
                name: "IX_Alertas_IdTipoAlerta",
                table: "Alertas");
        }
    }
}
