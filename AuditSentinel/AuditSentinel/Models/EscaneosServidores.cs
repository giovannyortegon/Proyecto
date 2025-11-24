namespace AuditSentinel.Models
{
    public class EscaneosServidores
    {
        public int IdServidor { get; set; }
        public Servidores Servidores { get; set; }

        public int IdEscaneo { get; set; }
        public Escaneos Escaneos { get; set; }
    }
}
