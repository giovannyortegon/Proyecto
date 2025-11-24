namespace AuditSentinel.Models
{
    public class PlantillasVulnerabilidades
    {
        public int IdPlantilla { get; set; }
        public Plantillas Plantillas { get; set; }
        public int IdVulnerabilidad { get; set; }
        public Vulnerabilidades Vulnerabilidades { get; set; }
    }
}
