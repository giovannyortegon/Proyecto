namespace AuditSentinel.Models
{
    public class EscaneosReportes
    {
        public int IdEscaneo { get; set; }
        public Escaneos Escaneos { get; set; }
        public int IdReporte { get; set; }
        public Reportes Reportes { get; set; }
    }
}
