using Microsoft.AspNetCore.SignalR;
using AuditSentinel.Services;

namespace AuditSentinel.Hubs
{
    public class EscaneoHub : Hub
    {
        public async Task JoinScanGroup(int escaneoId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Escaneo_{escaneoId}");
        }

        public bool IsServerOnline(string hostname)
        {
            return ScannerServerService.AgentesOnline.ContainsKey(hostname);
        }

        // Notifica a los clientes sobre el progreso y nuevas detecciones
        public async Task SendUpdate(int escaneoId, int porcentaje, string fase, string mensaje)
        {
            await Clients.Group($"Escaneo_{escaneoId}").SendAsync("ReceiveUpdate", new
            {
                porcentaje,
                fase,
                mensaje
            });
        }
    }
}