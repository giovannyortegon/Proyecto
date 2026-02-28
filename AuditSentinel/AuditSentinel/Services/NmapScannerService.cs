using System.Diagnostics;
using System.Text.RegularExpressions;
using AuditSentinel.Data;
using AuditSentinel.Models;

namespace AuditSentinel.Services
{
    public interface INmapScannerService
    {
        Task EjecutarEscaneoAsync(int escaneoId, string targetIp);
        int ObtenerProgreso(int escaneoId);

        public void DetenerEscaneo(int escaneoId);
    }

    public class NmapScannerService : INmapScannerService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private static readonly Dictionary<int, int> _progresos = new();
        private static readonly Dictionary<int, Process> _procesosActivos = new();

        public NmapScannerService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public int ObtenerProgreso(int escaneoId) =>
            _progresos.TryGetValue(escaneoId, out var p) ? p : 0;

        // NUEVO: Método para detener el proceso
        public void DetenerEscaneo(int escaneoId)
        {
            if (_procesosActivos.TryGetValue(escaneoId, out var proceso))
            {
                try {
                    if (!proceso.HasExited) proceso.Kill(true);
                } finally {
                    _procesosActivos.Remove(escaneoId);
                    _progresos[escaneoId] = 0;
                }
            }
        }
        public async Task EjecutarEscaneoAsync(int escaneoId, string targetIp)
        {
            _progresos[escaneoId] = 0;

            var startInfo = new ProcessStartInfo
            {
                FileName = "nmap",
                Arguments = $"-F -sV --stats-every 2s {targetIp}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var proceso = new Process { StartInfo = startInfo };

            try
            {
                proceso.Start();
                _progresos[escaneoId] = 5;

                // Leer salida estándar para el progreso
                while (!proceso.StandardOutput.EndOfStream)
                {
                    var linea = await proceso.StandardOutput.ReadLineAsync();
                    if (string.IsNullOrEmpty(linea)) continue;

                    var match = Regex.Match(linea, @"About (\d+\.\d+)% done");
                    if (match.Success)
                    {
                        _progresos[escaneoId] = (int)float.Parse(match.Groups[1].Value);
                    }
                }

                await proceso.WaitForExitAsync();
                _progresos[escaneoId] = 100;
            }
            catch (Exception ex)
            {
                // Usamos un Scope para guardar el error en la DB desde un servicio background
                using var scope = _scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();

                context.LogErroresEscaneos.Add(new LogErroresEscaneo
                {
                    EscaneoId = escaneoId,
                    Fase = "Motor Nmap",
                    Mensaje = ex.Message,
                    ComandoEjecutado = startInfo.Arguments
                });
                await context.SaveChangesAsync();
            }
        }
    }
}
