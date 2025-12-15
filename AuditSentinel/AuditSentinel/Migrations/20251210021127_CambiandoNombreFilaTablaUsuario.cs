using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuditSentinel.Migrations
{
    /// <inheritdoc />
    public partial class CambiandoNombreFilaTablaUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FechaEscaneo",
                table: "Usuarios",
                newName: "FechaCreado");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FechaCreado",
                table: "Usuarios",
                newName: "FechaEscaneo");
        }
    }
}
