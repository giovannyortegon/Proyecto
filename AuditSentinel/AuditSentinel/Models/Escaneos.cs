using System.ComponentModel.DataAnnotations;

namespace AuditSentinel.Models
{
    public enum EstadoEscaneo
    {
        Pendiente,
        EnProgreso,
        Completado,
        Fallido
    }
    public class Escaneos
    {
        [Key]
        public int IdEscaneo { get; set; }
        [Required]
        [MaxLength(200)]
        public string NombreEscaneo { get; set; }
        public EstadoEscaneo Estado { get; set; }  
        public DateTime FechaEscaneo { get; set; } = DateTime.Now;

        public ICollection<EscaneosServidores> EscaneosServidores { get; set; }
        public ICollection<EscaneosPlantillas> EscaneosPlantillas { get; set; }
        public ICollection<EscaneosReportes> EscaneosReportes { get; set; }
        public ICollection<EscaneosVulnerabilidades> EscaneosVulnerabilidades { get; set; }
    }
}
