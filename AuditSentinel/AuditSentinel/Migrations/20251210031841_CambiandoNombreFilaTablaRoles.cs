using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuditSentinel.Migrations
{
    /// <inheritdoc />
    public partial class CambiandoNombreFilaTablaRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IdRole",
                table: "Roles",
                newName: "IdRol");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IdRol",
                table: "Roles",
                newName: "IdRole");
        }
    }
}
