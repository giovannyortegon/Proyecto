using System.Text;
using AuditSentinel.Models;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace AuditSentinel.Services
{
    public class ExportService
    {
        private readonly GraficaService _graficaService;

        public ExportService(GraficaService graficaService)
        {
            _graficaService = graficaService;
        }

        // ================================
        // ESCANEOS - LISTA
        // ================================

        public void ExportToCsv(List<Escaneos> results, string filePath)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Id,Nombre,Estado,Fecha");
            foreach (var r in results)
                sb.AppendLine($"{r.IdEscaneo},{r.NombreEscaneo},{r.Estado},{r.FechaEscaneo:yyyy-MM-dd HH:mm}");
            File.WriteAllText(filePath, sb.ToString());
        }

        public void ExportToHtml(List<Escaneos> results, string filePath)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<html><head><title>Reporte Escaneos</title></head><body>");
            sb.AppendLine("<h1>Resultados de Escaneos</h1>");
            sb.AppendLine("<table border='1'><tr><th>Id</th><th>Nombre</th><th>Estado</th><th>Fecha</th></tr>");
            foreach (var r in results)
                sb.AppendLine($"<tr><td>{r.IdEscaneo}</td><td>{r.NombreEscaneo}</td><td>{r.Estado}</td><td>{r.FechaEscaneo:yyyy-MM-dd HH:mm}</td></tr>");
            sb.AppendLine("</table></body></html>");
            File.WriteAllText(filePath, sb.ToString());
        }

        public void ExportToPdf(List<Escaneos> results, string filePath)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(20);
                    page.Header().Text("Resultados de Escaneos").FontSize(20).Bold();
                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(50);
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                        });
                        table.Header(header =>
                        {
                            header.Cell().Text("Id").Bold();
                            header.Cell().Text("Nombre").Bold();
                            header.Cell().Text("Estado").Bold();
                            header.Cell().Text("Fecha").Bold();
                        });
                        foreach (var r in results)
                        {
                            table.Cell().Text(r.IdEscaneo.ToString());
                            table.Cell().Text(r.NombreEscaneo);
                            table.Cell().Text(r.Estado.ToString());
                            table.Cell().Text(r.FechaEscaneo.ToString("yyyy-MM-dd HH:mm"));
                        }
                    });
                });
            });
            document.GeneratePdf(filePath);
        }

        // ================================
        // ESCANEOS - INDIVIDUAL
        // ================================

        public void ExportEscaneoToCsv(Escaneos escaneo, string filePath)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Id,Nombre,Estado,Fecha");
            sb.AppendLine($"{escaneo.IdEscaneo},{escaneo.NombreEscaneo},{escaneo.Estado},{escaneo.FechaEscaneo:yyyy-MM-dd HH:mm}");
            File.WriteAllText(filePath, sb.ToString());
        }

        public void ExportEscaneoToHtml(Escaneos escaneo, string filePath)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<html><head><title>Reporte Escaneo</title></head><body>");
            sb.AppendLine($"<h1>Reporte Escaneo: {escaneo.NombreEscaneo}</h1><ul>");
            sb.AppendLine($"<li><b>ID:</b> {escaneo.IdEscaneo}</li>");
            sb.AppendLine($"<li><b>Estado:</b> {escaneo.Estado}</li>");
            sb.AppendLine($"<li><b>Fecha:</b> {escaneo.FechaEscaneo:yyyy-MM-dd HH:mm}</li>");
            sb.AppendLine("</ul></body></html>");
            File.WriteAllText(filePath, sb.ToString());
        }

        public void ExportEscaneoToPdf(Escaneos escaneo, string filePath)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(20);
                    page.Header().Text($"Reporte Escaneo: {escaneo.NombreEscaneo}").FontSize(20).Bold();
                    page.Content().Column(col =>
                    {
                        col.Item().Text($"ID: {escaneo.IdEscaneo}");
                        col.Item().Text($"Estado: {escaneo.Estado}");
                        col.Item().Text($"Fecha: {escaneo.FechaEscaneo:yyyy-MM-dd HH:mm}");
                    });
                });
            });
            document.GeneratePdf(filePath);
        }

        // ================================
        // REPORTES - LISTA
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