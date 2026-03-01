using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using AuditSentinel.Data;
using AuditSentinel.Models;
using Microsoft.EntityFrameworkCore;
using System.Buffers.Binary;

namespace AuditSentinel.Services
{
    public class ScannerServerService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ScannerServerService> _logger;
        private const int Port = 5001;

        public ScannerServerService(IServiceProvider serviceProvider, ILogger<ScannerServerService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            TcpListener listener = new TcpListener(IPAddress.Any, Port);
            listener.Start();
            _logger.LogInformation($"Servidor de Escaneo iniciado en el puerto {Port}...");

            while (!stoppingToken.IsCancellationRequested)
            {
                var client = await listener.AcceptTcpClientAsync(stoppingToken);
                _ = HandleAgentAsync(client, stoppingToken);
            }
        }

        private async Task HandleAgentAsync(TcpClient client, CancellationToken ct)
        {
            using (client)
            using (var stream = client.GetStream())
            {
                try
                {
                    // 1. Recibir saludo inicial del agente
                    var initialDoc = await ReceiveJsonDocumentAsync(stream);
                    string hostname = initialDoc.RootElement.GetProperty("hostname").GetString();
                    Console.WriteLine($"Agente conectado desde: {hostname}");

                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var context = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();

                        // 2. Buscar el escaneo pendiente
                        var escaneo = await context.Escaneos
                            .Include(e => e.EscaneosPlantillas)
                                .ThenInclude(ep => ep.Plantillas)
                                    .ThenInclude(p => p.PlantillasVulnerabilidades)
                                        .ThenInclude(pv => pv.Vulnerabilidades)
                            .FirstOrDefaultAsync(e => e.Estado == EstadoEscaneo.Pendiente);

                        if (escaneo != null)
                        {
                            escaneo.Estado = EstadoEscaneo.EnProgreso;
                            await context.SaveChangesAsync();

                            foreach (var ep in escaneo.EscaneosPlantillas)
                            {
                                foreach (var pv in ep.Plantillas.PlantillasVulnerabilidades)
                                {
                                    var v = pv.Vulnerabilidades;

                                    // 3. Enviar comando al agente Python
                                    var commandObj = new
                                    {
                                        type = "exec",
                                        comando = v.Comando,
                                        vulnId = v.IdVulnerabilidad,
                                        escaneoId = escaneo.IdEscaneo,
                                        pattern = v.ResultadoEsperado,
                                        timeoutSec = 60
                                    };
                                    await SendJsonAsync(stream, commandObj);

                                    // 4. Recibir y procesar resultado
                                    var responseDoc = await ReceiveJsonDocumentAsync(stream);
                                    JsonElement root = responseDoc.RootElement;

                                    // Aquí usamos GetProperty correctamente sobre JsonElement
                                    bool isMatched = root.GetProperty("matched").GetBoolean();
                                    int exitCode = root.GetProperty("exitCode").GetInt32();

                                    // 5. Guardar resultado en EscaneosVulnerabilidades
                                    var resultado = new EscaneosVulnerabilidades
                                    {
                                        IdEscaneo = escaneo.IdEscaneo,
                                        IdVulnerabilidad = v.IdVulnerabilidad,
                                        estado = isMatched ? Estado.Activa : Estado.Inactiva,
                                        FechaEscaneo = DateTime.Now
                                    };

                                    context.EscaneosVulnerabilidades.Add(resultado);
                                }
                            }
                            escaneo.Estado = EstadoEscaneo.Completado;
                            await context.SaveChangesAsync();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error de comunicación: {ex.Message}");
                }
            }
        }

        // Métodos auxiliares de lectura/escritura de Sockets
        private async Task<JsonDocument> ReceiveJsonDocumentAsync(NetworkStream stream)
        {
            byte[] header = new byte[4];
            await stream.ReadExactlyAsync(header, 0, 4);
            if (BitConverter.IsLittleEndian) Array.Reverse(header); // Ajuste de Endianness para Python (!I)
            int length = BitConverter.ToInt32(header, 0);

            byte[] buffer = new byte[length];
            await stream.ReadExactlyAsync(buffer, 0, length);

            return JsonDocument.Parse(Encoding.UTF8.GetString(buffer));
        }
        private async Task SendJsonAsync(NetworkStream stream, object obj)
        {
            byte[] jsonBytes = JsonSerializer.SerializeToUtf8Bytes(obj);
            byte[] header = BitConverter.GetBytes(jsonBytes.Length);
            if (BitConverter.IsLittleEndian) Array.Reverse(header); // Enviar como Big-Endian (!I)

            await stream.WriteAsync(header, 0, 4);
            await stream.WriteAsync(jsonBytes, 0, jsonBytes.Length);
        }

        private async Task<JsonElement?> ReceiveJsonAsync(NetworkStream stream)
        {
            byte[] header = new byte[4];
            await stream.ReadAsync(header, 0, 4);
            int length = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(header, 0));

            byte[] payload = new byte[length];
            int totalRead = 0;
            while (totalRead < length)
            {
                totalRead += await stream.ReadAsync(payload, totalRead, length - totalRead);
            }

            var json = Encoding.UTF8.GetString(payload);
            return JsonSerializer.Deserialize<JsonElement>(json);
        }
    }
}