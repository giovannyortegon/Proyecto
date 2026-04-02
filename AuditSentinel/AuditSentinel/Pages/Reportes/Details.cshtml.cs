using AuditSentinel.Data;
using AuditSentinel.Models;
using AuditSentinel.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace AuditSentinel.Pages.Reportes
{
    [Authorize(Roles = "Analista,Administrador,Auditor")]
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDBContext _context;
        private readonly GraficaService _graficaService;

        public DetailsModel(ApplicationDBContext context, GraficaService graficaService)
        {
            _context = context;
            _graficaService = graficaService;
        }

        public AuditSentinel.Models.Reportes Reporte { get; set; } = new();

        // Ruta web de la gr�fica para mostrar en pantalla
        public string RutaGraficaWeb { get; set; } = string.Empty;

        // ================================
        // CARGAR P�GINA � genera gr�fica
        // ================================
        public async Task<IActionResult> OnGetAsync(int id)
        {
            Reporte = await _context.Reportes
                .Include(r => r.EscaneosReportes)
                .ThenInclude(er => er.Escaneos)
                .FirstOrDefaultAsync(r => r.IdReporte == id);

            if (Reporte == null) return NotFound();

            // Generar gr�fica y guardarla en wwwroot/img/
            try
            {
                // Usamos un nombre de archivo �nico por reporte para la vista web
                string nombreArchivo = $"grafica_reporte_{Reporte.IdReporte}.png";
                string rutaFisica = Path.Combine(
                    Directory.GetCurrentDirectory(), "wwwroot", "img", nombreArchivo);

                // Reutilizamos GenerarGraficaReporte pero guardamos tambi�n en wwwroot
                _graficaService.GenerarGraficaReporteWeb(Reporte, rutaFisica);

                RutaGraficaWeb = $"/img/{nombreArchivo}?t={DateTimeOffset.UtcNow.ToUnixTimeSeconds()}";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Advertencia: no se pudo generar la gr�fica: {ex.Message}");
                RutaGraficaWeb = string.Empty;
            }

            return Page();
        }

        // ================================
        // EXPORTAR PDF CON GR�FICA
        // ================================
        public async Task<IActionResult> OnGetExportarPdfAsync(int id)
        {
            var reporte = await _context.Reportes
                .Include(r => r.EscaneosReportes)
                .ThenInclude(er => er.Escaneos)
                .FirstOrDefaultAsync(r => r.IdReporte == id);

            if (reporte == null) return NotFound();

            // Generar gr�fica para el PDF (ruta temporal)
            string graficaPath = string.Empty;
            try
            {
                graficaPath = _graficaService.GenerarGraficaReporte(reporte);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Advertencia: no se pudo generar la gr�fica: {ex.Message}");
            }

            var filePath = Path.Combine(Path.GetTempPath(), $"Reporte_{id}.pdf");

            var document = QuestPDF.Fluent.Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);

                    page.Size(595, 842);

                    page.Size(QuestPDF.Helpers.PageSizes.A4);


                    // -- ENCABEZADO --
                    page.Header().Column(col =>
                    {
                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text("AuditSentinel")
                                    .FontSize(10).FontColor("#666666");
                                c.Item().Text($"Informe de Auditor�a: {reporte.NombreReporte}")
                                    .FontSize(18).Bold().FontColor("#0B3D91");
                            });
                            row.ConstantItem(120).AlignRight().Column(c =>
                            {
                                c.Item().Text($"REP-{reporte.IdReporte:D4}")
                                    .FontSize(11).Bold();
                                c.Item().Text(reporte.Creado.ToString("dd/MM/yyyy"))
                                    .FontSize(9).FontColor("#888888");
                            });
                        });
                        col.Item().PaddingTop(5).LineHorizontal(1).LineColor("#0B3D91");
                    });

                    page.Content().PaddingTop(15).Column(col =>
                    {
                        // -- RESUMEN DE CUMPLIMIENTO --
                        col.Item().Text("Resumen de Cumplimiento")
                            .FontSize(13).Bold().FontColor("#0B3D91");

                        col.Item().PaddingTop(6).PaddingBottom(10).Row(row =>
                        {
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text($"Estado: {reporte.cumplimiento}")
                                    .FontSize(11).Bold();
                                c.Item().Text($"Referencia: REP-{reporte.IdReporte:D4}")
                                    .FontSize(10).FontColor("#555555");
                                c.Item().Text($"Fecha de creaci�n: {reporte.Creado:f}")
                                    .FontSize(10).FontColor("#555555");
                            });
                        });

                        col.Item().LineHorizontal(0.5f).LineColor("#CCCCCC");
                        col.Item().PaddingTop(12);

                        // -- GR�FICA DE TORTA --
                        if (!string.IsNullOrEmpty(graficaPath) && System.IO.File.Exists(graficaPath))
                        {
                            col.Item().Text("Distribuci�n de Estados de Escaneos")
                                .FontSize(13).Bold().FontColor("#0B3D91");

                            col.Item().PaddingTop(8).PaddingBottom(12)
                                .AlignCenter()
                                .Width(280)
                                .Image(graficaPath);

                            col.Item().LineHorizontal(0.5f).LineColor("#CCCCCC");
                            col.Item().PaddingTop(12);
                        }

                        // -- TABLA DE ESCANEOS --
                        col.Item().Text("Evidencia T�cnica � Escaneos Asociados")
                            .FontSize(13).Bold().FontColor("#0B3D91");

                        col.Item().PaddingTop(8).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(40);
                                columns.RelativeColumn(3);
                                columns.RelativeColumn(2);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Background("#0B3D91").Padding(5)
                                    .Text("ID").FontColor("#FFFFFF").Bold().FontSize(9);
                                header.Cell().Background("#0B3D91").Padding(5)
                                    .Text("Nombre del Escaneo").FontColor("#FFFFFF").Bold().FontSize(9);
                                header.Cell().Background("#0B3D91").Padding(5)
                                    .Text("Estado").FontColor("#FFFFFF").Bold().FontSize(9);
                            });

                            bool alterno = false;
                            foreach (var x in reporte.EscaneosReportes)
                            {
                                var bg = alterno ? "#F4F7FA" : "#FFFFFF";
                                alterno = !alterno;

                                table.Cell().Background(bg).Padding(5)
                                    .Text($"#{x.Escaneos?.IdEscaneo}").FontSize(9);
                                table.Cell().Background(bg).Padding(5)
                                    .Text(x.Escaneos?.NombreEscaneo ?? "-").FontSize(9);
                                table.Cell().Background(bg).Padding(5)
                                    .Text(x.Escaneos?.Estado.ToString() ?? "-").FontSize(9);
                            }
                        });
                    });

                    // -- PIE DE P�GINA --
                    page.Footer().AlignCenter()
                        .Text(txt =>
                        {
                            txt.Span("AuditSentinel � 2025 � P�gina ")
                                .FontSize(8).FontColor("#AAAAAA");
                            txt.CurrentPageNumber().FontSize(8).FontColor("#AAAAAA");
                            txt.Span(" de ").FontSize(8).FontColor("#AAAAAA");
                            txt.TotalPages().FontSize(8).FontColor("#AAAAAA");
                        });
                });
            });

            document.GeneratePdf(filePath);

            var bytes = System.IO.File.ReadAllBytes(filePath);
            return File(bytes, "application/pdf", $"Reporte_{reporte.NombreReporte}.pdf");
        }
    }
}

