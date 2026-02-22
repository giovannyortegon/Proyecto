using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuditSentinel.Migrations
{
    /// <inheritdoc />
    public partial class uniqnameenumSO : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Vulnerabilidades_NombreVulnerabilidad",
                table: "Vulnerabilidades",
                column: "NombreVulnerabilidad",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Servidores_IP",
                table: "Servidores",
                column: "IP",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Servidores_NombreServidor",
                table: "Servidores",
                column: "NombreServidor",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reportes_NombreReporte",
                table: "Reportes",
                column: "NombreReporte",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Plantillas_NombrePlantilla",
                table: "Plantillas",
                column: "NombrePlantilla",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Escaneos_NombreEscaneo",
                table: "Escaneos",
                column: "NombreEscaneo",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Vulnerabilidades_NombreVulnerabilidad",
                table: "Vulnerabilidades");

            migrationBuilder.DropIndex(
                name: "IX_Servidores_IP",
                table: "Servidores");

            migrationBuilder.DropIndex(
                name: "IX_Servidores_NombreServidor",
                table: "Servidores");

            migrationBuilder.DropIndex(
                name: "IX_Reportes_NombreReporte",
                table: "Reportes");

            migrationBuilder.DropIndex(
                name: "IX_Plantillas_NombrePlantilla",
                table: "Plantillas");

            migrationBuilder.DropIndex(
                name: "IX_Escaneos_NombreEscaneo",
                table: "Escaneos");
        }
    }
}
