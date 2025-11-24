using System.ComponentModel.DataAnnotations;

namespace AuditSentinel.Models
{
    public enum Cumplimiento
    {
        Cumple,
        NoCumple,
        ParcialmenteCumple
    }
    public class Reportes
    {
        [Key]
        public int IdReporte { get; set; }
        [Required]
        [MaxLength(100)]
        public string NombreReporte { get; set; }
        
        public Cumplimiento cumplimiento { get; set; }
        public DateTime Creado { get; set; } = DateTime.Now;

        public ICollection<EscaneosReportes> EscaneosReportes { get; set; }
    }
}
