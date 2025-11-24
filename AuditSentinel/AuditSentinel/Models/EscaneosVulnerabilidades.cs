namespace AuditSentinel.Models
{
    public enum Estado
    {
        Activa,
        Inactiva
    }
    public class EscaneosVulnerabilidades
    {
        public int IdEscaneo { get; set; }
        public Escaneos Escaneos { get; set; }
        public int IdVulnerabilidad { get; set; }
        public Vulnerabilidades Vulnerabilidades { get; set; }
        public Estado estado { get; set; }
        public DateTime FechaEscaneo { get; set; } = DateTime.Now;
    }
}
