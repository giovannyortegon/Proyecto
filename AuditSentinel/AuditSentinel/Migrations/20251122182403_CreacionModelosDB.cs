using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuditSentinel.Migrations
{
    /// <inheritdoc />
    public partial class CreacionModelosDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Escaneos",
                columns: table => new
                {
                    IdEscaneo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreEscaneo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaEscaneo = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Escaneos", x => x.IdEscaneo);
                });

            migrationBuilder.CreateTable(
                name: "Plantillas",
                columns: table => new
                {
                    IdPlantilla = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombrePlantilla = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Version = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plantillas", x => x.IdPlantilla);
                });

            migrationBuilder.CreateTable(
                name: "Reportes",
                columns: table => new
                {
                    IdReporte = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreReporte = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    cumplimiento = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Creado = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reportes", x => x.IdReporte);
                });

            migrationBuilder.CreateTable(
                name: "Servidores",
                columns: table => new
                {
                    IdServidor = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreServidor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IP = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    SistemaOperativo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Create_is = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Servidores", x => x.IdServidor);
                });

            migrationBuilder.CreateTable(
                name: "Vulnerabilidades",
                columns: table => new
                {
                    IdVulnerabilidad = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreVulnerabilidad = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NivelRiesgo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Comando = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ResultadoEsperado = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    FechaDetectada = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vulnerabilidades", x => x.IdVulnerabilidad);
                });

            migrationBuilder.CreateTable(
                name: "EscaneosPlantillas",
                columns: table => new
                {
                    IdEscaneo = table.Column<int>(type: "int", nullable: false),
                    IdPlantilla = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EscaneosPlantillas", x => new { x.IdEscaneo, x.IdPlantilla });
                    table.ForeignKey(
                        name: "FK_EscaneosPlantillas_Escaneos_IdEscaneo",
                        column: x => x.IdEscaneo,
                        principalTable: "Escaneos",
                        principalColumn: "IdEscaneo",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EscaneosPlantillas_Plantillas_IdPlantilla",
                        column: x => x.IdPlantilla,
                        principalTable: "Plantillas",
                        principalColumn: "IdPlantilla",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EscaneosReportes",
                columns: table => new
                {
                    IdEscaneo = table.Column<int>(type: "int", nullable: false),
                    IdReporte = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EscaneosReportes", x => new { x.IdEscaneo, x.IdReporte });
                    table.ForeignKey(
                        name: "FK_EscaneosReportes_Escaneos_IdEscaneo",
                        column: x => x.IdEscaneo,
                        principalTable: "Escaneos",
                        principalColumn: "IdEscaneo",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EscaneosReportes_Reportes_IdReporte",
                        column: x => x.IdReporte,
                        principalTable: "Reportes",
                        principalColumn: "IdReporte",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EscaneosServidores",
                columns: table => new
                {
                    IdServidor = table.Column<int>(type: "int", nullable: false),
                    IdEscaneo = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EscaneosServidores", x => new { x.IdEscaneo, x.IdServidor });
                    table.ForeignKey(
                        name: "FK_EscaneosServidores_Escaneos_IdEscaneo",
                        column: x => x.IdEscaneo,
                        principalTable: "Escaneos",
                        principalColumn: "IdEscaneo",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EscaneosServidores_Servidores_IdServidor",
                        column: x => x.IdServidor,
                        principalTable: "Servidores",
                        principalColumn: "IdServidor",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EscaneosVulnerabilidades",
                columns: table => new
                {
                    IdEscaneo = table.Column<int>(type: "int", nullable: false),
                    IdVulnerabilidad = table.Column<int>(type: "int", nullable: false),
                    estado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaEscaneo = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EscaneosVulnerabilidades", x => new { x.IdEscaneo, x.IdVulnerabilidad });
                    table.ForeignKey(
                        name: "FK_EscaneosVulnerabilidades_Escaneos_IdEscaneo",
                        column: x => x.IdEscaneo,
                        principalTable: "Escaneos",
                        principalColumn: "IdEscaneo",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EscaneosVulnerabilidades_Vulnerabilidades_IdVulnerabilidad",
                        column: x => x.IdVulnerabilidad,
                        principalTable: "Vulnerabilidades",
                        principalColumn: "IdVulnerabilidad",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlantillasVulnerabilidades",
                columns: table => new
                {
                    IdPlantilla = table.Column<int>(type: "int", nullable: false),
                    IdVulnerabilidad = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlantillasVulnerabilidades", x => new { x.IdPlantilla, x.IdVulnerabilidad });
                    table.ForeignKey(
                        name: "FK_PlantillasVulnerabilidades_Plantillas_IdPlantilla",
                        column: x => x.IdPlantilla,
                        principalTable: "Plantillas",
                        principalColumn: "IdPlantilla",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlantillasVulnerabilidades_Vulnerabilidades_IdVulnerabilidad",
                        column: x => x.IdVulnerabilidad,
                        principalTable: "Vulnerabilidades",
                        principalColumn: "IdVulnerabilidad",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EscaneosPlantillas_IdPlantilla",
                table: "EscaneosPlantillas",
                column: "IdPlantilla");

            migrationBuilder.CreateIndex(
                name: "IX_EscaneosReportes_IdReporte",
                table: "EscaneosReportes",
                column: "IdReporte");

            migrationBuilder.CreateIndex(
                name: "IX_EscaneosServidores_IdServidor",
                table: "EscaneosServidores",
                column: "IdServidor");

            migrationBuilder.CreateIndex(
                name: "IX_EscaneosVulnerabilidades_IdVulnerabilidad",
                table: "EscaneosVulnerabilidades",
                column: "IdVulnerabilidad");

            migrationBuilder.CreateIndex(
                name: "IX_PlantillasVulnerabilidades_IdVulnerabilidad",
                table: "PlantillasVulnerabilidades",
                column: "IdVulnerabilidad");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EscaneosPlantillas");

            migrationBuilder.DropTable(
                name: "EscaneosReportes");

            migrationBuilder.DropTable(
                name: "EscaneosServidores");

            migrationBuilder.DropTable(
                name: "EscaneosVulnerabilidades");

            migrationBuilder.DropTable(
                name: "PlantillasVulnerabilidades");

            migrationBuilder.DropTable(
                name: "Reportes");

            migrationBuilder.DropTable(
                name: "Servidores");

            migrationBuilder.DropTable(
                name: "Escaneos");

            migrationBuilder.DropTable(
                name: "Plantillas");

            migrationBuilder.DropTable(
                name: "Vulnerabilidades");
        }
    }
}
