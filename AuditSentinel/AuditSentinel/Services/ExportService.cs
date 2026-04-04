using AuditSentinel.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Text;

namespace AuditSentinel.Services
{
    public class ExportService
    {
        private readonly GraficaService _graficaService;

        private const string VerdeHeader = "#1e4d2b";
        private const string RojoCritico = "#d32f2f";
        private const string AzulMedio = "#1976d2";
        private const string VerdeBajo = "#388e3c";
        private const string NaranjaEstado = "#f9a825";

        public ExportService(GraficaService graficaService)
        {
            _graficaService = graficaService;
        }


        //public void ExportToCsv(List<Escaneos> results, string filePath)
        //{
        //    var sb = new StringBuilder();
        //    sb.AppendLine("Id,Nombre,Estado,Fecha");
        //    foreach (var r in results)
        //        sb.AppendLine($"{r.IdEscaneo},{r.NombreEscaneo},{r.Estado},{r.FechaEscaneo:yyyy-MM-dd HH:mm}");
        //    File.WriteAllText(filePath, sb.ToString());
        //}

        //public void ExportToHtml(List<Escaneos> results, string filePath)
        //{
        //    var sb = new StringBuilder();
        //    sb.AppendLine("<html><head><title>Reporte Escaneos</title></head><body>");
        //    sb.AppendLine("<h1>Resultados de Escaneos</h1>");
        //    sb.AppendLine("<table border='1'><tr><th>Id</th><th>Nombre</th><th>Estado</th><th>Fecha</th></tr>");
        //    foreach (var r in results)
        //        sb.AppendLine($"<tr><td>{r.IdEscaneo}</td><td>{r.NombreEscaneo}</td><td>{r.Estado}</td><td>{r.FechaEscaneo:yyyy-MM-dd HH:mm}</td></tr>");
        //    sb.AppendLine("</table></body></html>");
        //    File.WriteAllText(filePath, sb.ToString());
        //}

        // ================================
        // ESCANEO
        // ================================
        public void ExportEscaneoToPdf(Escaneos escaneo, string filePath)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(0); // Para que el encabezado verde llegue a los bordes
                    page.Size(PageSizes.A4);
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily(Fonts.SegoeUI));

                    // --- ENCABEZADO VERDE OSCURO ---
                    page.Header().Background(VerdeHeader).PaddingVertical(25).AlignCenter().Column(col =>
                    {
                        col.Item().Text("INFORME DE ESCANEOS").FontSize(26).Bold().FontColor(Colors.White);
                        col.Item().Text(escaneo.NombreEscaneo ?? "Auditoría de Red").FontSize(14).FontColor(Colors.White);
                        col.Item().Text("CVEs y Hallazgos").FontSize(12).FontColor(Colors.White).Italic();
                    });

                    // --- CUERPO DEL REPORTE ---
                    page.Content().PaddingHorizontal(40).PaddingVertical(30).Column(column =>
                    {
                        column.Item().Text("Detalle de Hallazgos por Servidor").FontSize(18).Bold().FontColor(Colors.Black);

                        // Resumen ejecutivo
                        column.Item().PaddingTop(10).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingBottom(10).Column(c => {
                            c.Item().Text("Resumen de Escaneo").FontSize(12).Bold().FontColor("#1a3a5a");
                            c.Item().Text($"Nombre de red: {escaneo.NombreEscaneo}");
                            c.Item().Text($"ID Operación: #{escaneo.IdEscaneo}");
                            c.Item().Text($"Fecha de ejecución: {escaneo.FechaEscaneo:dd 'de' MMMM 'de' yyyy}");
                        });

                        column.Item().PaddingVertical(15).Text("Vulnerabilidades Detectadas").FontSize(12).Bold().FontColor("#1a3a5a");

                        // --- TABLA PLANA DE HALLAZGOS (Como en la web) ---
                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2); // Servidor
                                columns.RelativeColumn(3); // Vulnerabilidad
                                columns.RelativeColumn(1); // Riesgo
                                columns.RelativeColumn(1); // Estado
                            });

                            // Cabecera de tabla
                            table.Header(header =>
                            {
                                header.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text("Servidor").Bold();
                                header.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text("Vulnerabilidad").Bold();
                                header.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(5).AlignCenter().Text("Riesgo").Bold();
                                header.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(5).AlignCenter().Text("Estado").Bold();
                            });

                            // Iteración plana sobre todos los hallazgos
                            if (escaneo.EscaneosVulnerabilidades != null && escaneo.EscaneosVulnerabilidades.Any())
                            {
                                // Obtenemos el nombre del servidor desde la lista general para evitar el error CS1061
                                var nombreServidor = escaneo.EscaneosServidores?.FirstOrDefault()?.Servidores?.NombreServidor ?? "Servidor";

                                foreach (var v in escaneo.EscaneosVulnerabilidades)
                                {
                                    var datosVuln = v.Vulnerabilidades;

                                    // 1. Columna Servidor
                                    table.Cell().Border(1).BorderColor(Colors.Grey.Lighten3).Padding(5).AlignMiddle()
                                         .Text(nombreServidor).FontSize(9);

                                    // 2. Columna Vulnerabilidad
                                    table.Cell().Border(1).BorderColor(Colors.Grey.Lighten3).Padding(5).AlignMiddle()
                                         .Text(datosVuln?.NombreVulnerabilidad ?? "Desconocida").FontSize(9);

                                    // 3. Columna Riesgo (Badge) - Corregido error CS0023
                                    string nivel = datosVuln != null ? datosVuln.NivelRiesgo.ToString() : "Bajo";
                                    var colorRiesgo = nivel.Contains("Critico") ? RojoCritico
                                                    : (nivel == "Medio" ? AzulMedio : VerdeBajo);

                                    table.Cell().Border(1).BorderColor(Colors.Grey.Lighten3).Padding(5).AlignMiddle().AlignCenter()
                                         .Background(colorRiesgo)
                                         .Text(nivel).FontColor(Colors.White).Bold().FontSize(9);

                                    // 4. Columna Estado (Badge) - Corregido error CS0023
                                    string estadoStr = v.estado.ToString();
                                    table.Cell().Border(1).BorderColor(Colors.Grey.Lighten3).Padding(5).AlignMiddle().AlignCenter()
                                         .Background(NaranjaEstado)
                                         .Text(estadoStr).FontColor(Colors.White).Bold().FontSize(9);
                                }
                            }
                            else
                            {
                                table.Cell().ColumnSpan(4).Padding(10).AlignCenter()
                                     .Text("No se detectaron vulnerabilidades en este escaneo.")
                                     .Italic().FontColor(Colors.Grey.Medium);
                            }
                        });
                    });

                    // --- PIE DE PÁGINA ---
                    page.Footer().PaddingHorizontal(40).PaddingBottom(20).Row(row =>
                    {
                        row.RelativeItem().Column(c => {
                            c.Item().Text($"Ejecutado: {escaneo.FechaEscaneo:dd/MM/yyyy}");
                            c.Item().Text("Preparado por: Equipo de Auditoría AuditSentinel");
                        });

                        row.RelativeItem().AlignRight().Column(c => {
                            c.Item().Text("AUDITSENTINEL").FontSize(14).Bold().FontColor(VerdeHeader);
                            c.Item().Text("Auditsentinel, S.A.").FontSize(10);
                            c.Item().Text(x => { x.Span("Página "); x.CurrentPageNumber(); });
                        });
                    });
                });
            });

            document.GeneratePdf(filePath);
        }
     

        // ================================
        // REPORTES
        // ================================

        public void ExportReportesToCsv(List<Reportes> items, string filePath)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Id,Nombre,Cumplimiento,Fecha");
            foreach (var r in items)
                sb.AppendLine($"{r.IdReporte},{r.NombreReporte},{r.cumplimiento},{r.Creado:yyyy-MM-dd HH:mm}");
            File.WriteAllText(filePath, sb.ToString());
        }

        public void ExportReportesToHtml(List<Reportes> items, string filePath)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<html><head><meta charset='utf-8'><title>Reportes de Auditoría</title>");
            sb.AppendLine("<style>body{font-family:Arial,sans-serif;padding:20px}table{border-collapse:collapse;width:100%}th,td{border:1px solid #ccc;padding:8px;text-align:left}th{background:#0B3D91;color:white}</style>");
            sb.AppendLine("</head><body>");
            sb.AppendLine("<h1>Reportes de Auditoría</h1>");
            sb.AppendLine("<table><tr><th>Id</th><th>Nombre</th><th>Cumplimiento</th><th>Escaneos</th><th>Fecha</th></tr>");
            foreach (var r in items)
                sb.AppendLine($"<tr><td>{r.IdReporte}</td><td>{r.NombreReporte}</td><td>{r.cumplimiento}</td><td>{r.EscaneosReportes?.Count ?? 0}</td><td>{r.Creado:yyyy-MM-dd HH:mm}</td></tr>");
            sb.AppendLine("</table></body></html>");
            File.WriteAllText(filePath, sb.ToString());
        }

        public void ExportReportesToPdf(List<Reportes> items, string filePath)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(20);
                    page.Header().Text("Reportes de Auditoría").FontSize(20).Bold();
                    page.Content().Column(col =>
                    {
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(40);
                                columns.RelativeColumn(3);
                                columns.RelativeColumn(2);
                                columns.ConstantColumn(60);
                                columns.RelativeColumn(2);
                            });
                            table.Header(header =>
                            {
                                header.Cell().Text("Id").Bold();
                                header.Cell().Text("Nombre").Bold();
                                header.Cell().Text("Cumplimiento").Bold();
                                header.Cell().Text("Escaneos").Bold();
                                header.Cell().Text("Fecha").Bold();
                            });
                            foreach (var r in items)
                            {
                                table.Cell().Text(r.IdReporte.ToString());
                                table.Cell().Text(r.NombreReporte);
                                table.Cell().Text(r.cumplimiento.ToString());
                                table.Cell().Text((r.EscaneosReportes?.Count ?? 0).ToString());
                                table.Cell().Text(r.Creado.ToString("yyyy-MM-dd HH:mm"));
                            }
                        });
                    });
                });
            });
            document.GeneratePdf(filePath);
        }
    }
}