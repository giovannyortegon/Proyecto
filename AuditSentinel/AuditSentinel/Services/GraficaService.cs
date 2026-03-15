#nullable enable
using System.Diagnostics;
using AuditSentinel.Data;
using AuditSentinel.Models;
using MatplotlibCS;
using MatplotlibCS.PlotItems;

namespace AuditSentinel.Services
{
    public class GraficaService
    {
        private readonly IWebHostEnvironment _env;
        private readonly ApplicationDBContext _context;

        public GraficaService(IWebHostEnvironment env, ApplicationDBContext context)
        {
            _env = env;
            _context = context;
        }

        // ================================
        // 1. GRÁFICA GLOBAL (dashboard)
        //    Cuenta todos los escaneos de la BD
        //    Guarda en wwwroot/img/grafica_escaneos.png
        // ================================
        public string GenerarGrafica()
        {
            var completado = _context.Escaneos.Count(e => e.Estado == EstadoEscaneo.Completado);
            var enProgreso = _context.Escaneos.Count(e => e.Estado == EstadoEscaneo.EnProgreso);
            var pendiente  = _context.Escaneos.Count(e => e.Estado == EstadoEscaneo.Pendiente);
            var fallido    = _context.Escaneos.Count(e => e.Estado == EstadoEscaneo.Fallido);

            string rutaImagen = Path.Combine(_env.WebRootPath, "img", "grafica_escaneos.png");
            EjecutarScript(rutaImagen, completado, enProgreso, pendiente, fallido);
            return rutaImagen;
        }

        // ================================
        // 2. GRÁFICA DE REPORTE PARA PDF
        //    Usa solo los escaneos del reporte
        //    Guarda en carpeta temporal
        // ================================
        public string GenerarGraficaReporte(Reportes reporte)
        {
            var (completado, enProgreso, pendiente, fallido) = ContarEstados(reporte);

            string rutaImagen = Path.Combine(
                Path.GetTempPath(), $"grafica_reporte_{reporte.IdReporte}.png");

            EjecutarScript(rutaImagen, completado, enProgreso, pendiente, fallido);
            return rutaImagen;
        }

        // ================================
        // 3. GRÁFICA DE REPORTE PARA WEB
        //    Usa solo los escaneos del reporte
        //    Guarda en wwwroot/img/ para mostrar en pantalla
        // ================================
        public void GenerarGraficaReporteWeb(Reportes reporte, string rutaFisica)
        {
            var (completado, enProgreso, pendiente, fallido) = ContarEstados(reporte);
            EjecutarScript(rutaFisica, completado, enProgreso, pendiente, fallido);
        }

        // ================================
        // RUTA WEB para <img src="...">
        // ================================
        public string RutaWebGrafica() => "/img/grafica_escaneos.png";

        // ================================
        // PRIVADO: contar estados del reporte
        // ================================
        private (int completado, int enProgreso, int pendiente, int fallido) ContarEstados(Reportes reporte)
        {
            var escaneos = reporte.EscaneosReportes
                .Where(er => er.Escaneos != null)
                .Select(er => er.Escaneos!)
                .ToList();

            return (
                escaneos.Count(e => e.Estado == EstadoEscaneo.Completado),
                escaneos.Count(e => e.Estado == EstadoEscaneo.EnProgreso),
                escaneos.Count(e => e.Estado == EstadoEscaneo.Pendiente),
                escaneos.Count(e => e.Estado == EstadoEscaneo.Fallido)
            );
        }

        // ================================
        // PRIVADO: ejecutar grafica.py
        // ================================
        private void EjecutarScript(string rutaSalida, int completado, int enProgreso, int pendiente, int fallido)
        {
            string scriptPath = Path.Combine(_env.ContentRootPath, "Scripts", "grafica.py");

            var dir = Path.GetDirectoryName(rutaSalida);
            if (!string.IsNullOrEmpty(dir))
                Directory.CreateDirectory(dir);

            var psi = new ProcessStartInfo
            {
                FileName               = "python",
                Arguments              = $"\"{scriptPath}\" \"{rutaSalida}\" {completado} {enProgreso} {pendiente} {fallido}",
                UseShellExecute        = false,
                CreateNoWindow         = true,
                RedirectStandardOutput = true,
                RedirectStandardError  = true
            };

            using var process = new Process { StartInfo = psi };
            process.Start();

            string stderr = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (process.ExitCode != 0)
                throw new Exception($"Error en grafica.py (exit {process.ExitCode}):\n{stderr}");
        }
    }
}