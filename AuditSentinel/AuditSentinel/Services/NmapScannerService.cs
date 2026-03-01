using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using AuditSentinel.Data;
using AuditSentinel.Models;

namespace AuditSentinel.Services
{
    public interface INmapScannerService
    {
        Task EjecutarEscaneoAsync(int escaneoId, string targetIp);
        int ObtenerProgreso(int escaneoId);
        void DetenerEscaneo(int escaneoId);

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

        public void DetenerEscaneo(int escaneoId)
        {
            if (_procesosActivos.TryGetValue(escaneoId, out var proceso))
            {
                try
                {
                    if (!proceso.HasExited) proceso.Kill(true);
                }
                finally
                {
                    LimpiarProceso(escaneoId);
                }
            }
        }

        public async Task EjecutarEscaneoAsync(int escaneoId, string targetIp)
        {
            _progresos[escaneoId] = 0;

            // 1. Marcar inicio en la DB
            using (var scope = _scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
                var registro = await db.Escaneos.FindAsync(escaneoId);
                if (registro != null)
                {
                    registro.Estado = EstadoEscaneo.EnProgreso;
                    await db.SaveChangesAsync();
                }
            }

            var startInfo = new ProcessStartInfo
            {
                FileName = "nmap",
                // --noninteractive evita bloqueos y --stats-every fuerza reportes de progreso
                Arguments = $"-F -sV --stats-every 2s --noninteractive {targetIp}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var proceso = new Process { StartInfo = startInfo };
            _procesosActivos[escaneoId] = proceso; // Permitir detenerlo después 

            try
            {
                proceso.Start();
                _progresos[escaneoId] = 5;

                while (!proceso.StandardOutput.EndOfStream)
                {
                    var linea = await proceso.StandardOutput.ReadLineAsync();
                    if (string.IsNullOrEmpty(linea)) continue;

                    // Regex mejorado para capturar el progreso real de Nmap
                    var match = Regex.Match(linea, @"About (\d+(\.\d+)?)% done");
                    if (match.Success)
                    {
                        _progresos[escaneoId] = (int)float.Parse(match.Groups[1].Value);
                    }
                }

                await proceso.WaitForExitAsync();

                // 2. Marcar finalización en la DB
                using (var scope = _scopeFactory.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
                    var registro = await db.Escaneos.FindAsync(escaneoId);
                    if (registro != null)
                    {
                        registro.Estado = EstadoEscaneo.Completado;
                        await db.SaveChangesAsync();
                    }
                }
                _progresos[escaneoId] = 100;
            }
            catch (Exception ex)
            {
                //[cite_start]// Registro de error en LogErroresEscaneo [cite: 3]
                _progresos[escaneoId] = 0;
                using var scope = _scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();

                context.LogErroresEscaneos.Add(new LogErroresEscaneo
                {
                    EscaneoId = escaneoId,
                    Fase = "Motor Nmap",
                    Mensaje = ex.Message,
                    ComandoEjecutado = startInfo.Arguments
                });

                var registro = await context.Escaneos.FindAsync(escaneoId);
                if (registro != null) registro.Estado = EstadoEscaneo.Fallido;
                await context.SaveChangesAsync();
            }
            finally
            {
                _procesosActivos.Remove(escaneoId);
            }
        }

        private async Task GuardarErrorEnDB(int escaneoId, string fase, string mensaje, string comando)
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();

            context.LogErroresEscaneos.Add(new LogErroresEscaneo
            {
                EscaneoId = escaneoId,
                Fase = fase,
                Mensaje = string.IsNullOrWhiteSpace(mensaje) ? "Error desconocido de Nmap" : mensaje,
                ComandoEjecutado = comando,
                FechaError = DateTime.Now
            });
            await context.SaveChangesAsync();
        }

        private void LimpiarProceso(int escaneoId)
        {
            _procesosActivos.Remove(escaneoId);
            if (_progresos.ContainsKey(escaneoId) && _progresos[escaneoId] < 100)
                _progresos[escaneoId] = 0;
        }
    }
}