using AuditSentinel.Data;
using AuditSentinel.Hubs;
using AuditSentinel.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Buffers.Binary;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;

namespace AuditSentinel.Services
{
    public class ScannerServerService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IHubContext<EscaneoHub> _hubContext;
        private const int Port = 5001;

        public static ConcurrentDictionary<string, bool> AgentesOnline = new ConcurrentDictionary<string, bool>();
        public static ConcurrentDictionary<int, CancellationTokenSource> EscaneosEnCurso = new ConcurrentDictionary<int, CancellationTokenSource>();

        public ScannerServerService(IServiceProvider serviceProvider, IHubContext<EscaneoHub> hubContext)
        {
            _serviceProvider = serviceProvider;
            _hubContext = hubContext;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            TcpListener listener = new TcpListener(IPAddress.Any, Port);
            listener.Start();

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var client = await listener.AcceptTcpClientAsync(stoppingToken);
                    _ = HandleAgentAsync(client, stoppingToken);
                }
                catch { }
            }
        }

        private async Task HandleAgentAsync(TcpClient client, CancellationToken ct)
        {
            string currentHost = "Desconocido";
            using (client)
            using (NetworkStream stream = client.GetStream()) // Aquí se define stream
            {
                try
                {
                    var handshake = await ReceiveJsonDocumentAsync(stream);
                    currentHost = handshake.RootElement.GetProperty("hostname").GetString();
                    AgentesOnline.TryAdd(currentHost, true);
                    await _hubContext.Clients.All.SendAsync("AgentStatus", currentHost, true);

                    while (client.Connected && !ct.IsCancellationRequested)
                    {
                        using (var scope = _serviceProvider.CreateScope())
                        {
                            var context = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();

                            // 1. Solo buscar escaneos que estén en estado "Pendiente"
                            // Importante: No debe haber escaneos creados directamente en "Pendiente" si no quieres que inicien solos.
                            var escaneo = await context.Escaneos
                                .Include(e => e.EscaneosPlantillas).ThenInclude(ep => ep.Plantillas)
                                    .ThenInclude(p => p.PlantillasVulnerabilidades).ThenInclude(pv => pv.Vulnerabilidades)
                                .FirstOrDefaultAsync(e => e.Estado == EstadoEscaneo.Pendiente);

                            if (escaneo != null)
                            {
                                // Cambiamos a EnProgreso para que ningún otro hilo lo tome
                                escaneo.Estado = EstadoEscaneo.EnProgreso;
                                await context.SaveChangesAsync();

                                var pruebas = escaneo.EscaneosPlantillas
                                    .SelectMany(p => p.Plantillas.PlantillasVulnerabilidades)
                                    .Select(pv => pv.Vulnerabilidades).ToList();

                                for (int i = 0; i < pruebas.Count; i++)
                                {
                                    var v = pruebas[i];

                                    await SendJsonAsync(stream, new
                                    {
                                        type = "exec",
                                        comando = v.Comando,
                                        pattern = v.ResultadoEsperado,
                                        vulnId = v.IdVulnerabilidad,
                                        escaneoId = escaneo.IdEscaneo
                                    });

                                    var response = await ReceiveJsonDocumentAsync(stream);
                                    bool matched = response.RootElement.GetProperty("matched").GetBoolean();

                                    var resultado = new EscaneosVulnerabilidades
                                    {
                                        IdEscaneo = escaneo.IdEscaneo,
                                        IdVulnerabilidad = v.IdVulnerabilidad,
                                        estado = matched ? Estado.Activa : Estado.Inactiva,
                                        FechaEscaneo = DateTime.Now
                                    };

                                    context.EscaneosVulnerabilidades.Add(resultado);
                                    await context.SaveChangesAsync();

                                    // Notificar progreso
                                    int progreso = (int)((double)(i + 1) / pruebas.Count * 100);
                                    await _hubContext.Clients.Group($"Escaneo_{escaneo.IdEscaneo}").SendAsync("ReceiveUpdate", new
                                    {
                                        porcentaje = progreso,
                                        ultimaVuln = v.NombreVulnerabilidad,
                                        estado = matched ? "Activa" : "Inactiva",
                                        finalizado = (progreso == 100) // Enviamos bandera de fin
                                    });
                                }

                                // 2. ACTUALIZAR A COMPLETADO AL FINALIZAR
                                escaneo.Estado = EstadoEscaneo.Completado;
                                await context.SaveChangesAsync();
                            }
                        }
                        await Task.Delay(3000); // Esperar antes de buscar el siguiente escaneo pendiente
                    }
                }
                catch { }
                finally
                {
                    AgentesOnline.TryRemove(currentHost, out _);
                    await _hubContext.Clients.All.SendAsync("AgentStatus", currentHost, false);
                }
            }
        }

        private async Task SendJsonAsync(NetworkStream s, object obj)
        {
            byte[] data = JsonSerializer.SerializeToUtf8Bytes(obj);
            byte[] header = new byte[4];
            BinaryPrimitives.WriteInt32BigEndian(header, data.Length);
            await s.WriteAsync(header, 0, 4);
            await s.WriteAsync(data, 0, data.Length);
        }

        private async Task<JsonDocument> ReceiveJsonDocumentAsync(NetworkStream s)
        {
            byte[] header = new byte[4];
            await s.ReadExactlyAsync(header, 0, 4);
            int len = BinaryPrimitives.ReadInt32BigEndian(header);
            byte[] buffer = new byte[len];
            await s.ReadExactlyAsync(buffer, 0, len);
            return JsonDocument.Parse(System.Text.Encoding.UTF8.GetString(buffer));
        }
    }
}