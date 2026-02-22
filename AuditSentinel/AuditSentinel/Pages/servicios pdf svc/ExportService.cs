using System.Text;
using AuditSentinel.Models;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace AuditSentinel.Services
{
    public class ExportService
    {
        // Exportación general (lista de escaneos)
        public void ExportToCsv(List<Escaneos> results, string filePath)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Id,Nombre,Estado,Fecha");

            foreach (var r in results)
            {
                sb.AppendLine($"{r.IdEscaneo},{r.NombreEscaneo},{r.Estado},{r.FechaEscaneo:yyyy-MM-dd HH:mm}");
            }

            File.WriteAllText(filePath, sb.ToString());
        }

        public void ExportToHtml(List<Escaneos> results, string filePath)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<html><head><title>Reporte Escaneos</title></head><body>");
            sb.AppendLine("<h1>Resultados de Escaneos</h1>");
            sb.AppendLine("<table border='1'>");
            sb.AppendLine("<tr><th>Id</th><th>Nombre</th><th>Estado</th><th>Fecha</th></tr>");

            foreach (var r in results)
            {
                sb.AppendLine($"<tr><td>{r.IdEscaneo}</td><td>{r.NombreEscaneo}</td><td>{r.Estado}</td><td>{r.FechaEscaneo:yyyy-MM-dd HH:mm}</td></tr>");
            }

            sb.AppendLine("</table></body></html>");
            File.WriteAllText(filePath, sb.ToString());
        }

        public void ExportToPdf(List<Escaneos> results, string filePath)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
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
                            header.Cell().Text("Id");
                            header.Cell().Text("Nombre");
                            header.Cell().Text("Estado");
                            header.Cell().Text("Fecha");
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

        // Exportación individual (un escaneo específico)
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
            sb.AppendLine($"<h1>Reporte Escaneo: {escaneo.NombreEscaneo}</h1>");
            sb.AppendLine("<ul>");
            sb.AppendLine($"<li><b>ID:</b> {escaneo.IdEscaneo}</li>");
            sb.AppendLine($"<li><b>Estado:</b> {escaneo.Estado}</li>");
            sb.AppendLine($"<li><b>Fecha:</b> {escaneo.FechaEscaneo:yyyy-MM-dd HH:mm}</li>");
            sb.AppendLine("</ul>");
            sb.AppendLine("</body></html>");
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
    }
}