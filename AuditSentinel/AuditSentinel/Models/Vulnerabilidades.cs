using System.ComponentModel.DataAnnotations;

namespace AuditSentinel.Models
{
    public enum NivelRiesgo
    {
        Bajo,
        Medio,
        Alto,
        Critico
    }
    public class Vulnerabilidades
    {
        [Key]
        public int IdVulnerabilidad { get; set; }
        [Required]
        [MaxLength(200)]
        public string NombreVulnerabilidad { get; set; }
        public NivelRiesgo NivelRiesgo { get; set; }
        [MaxLength(200)]
        public string Descripcion { get; set; }
        [MaxLength(500)]
        public string Comando { get; set; }
        [MaxLength(200)]
        public string ResultadoEsperado { get; set; }
        public DateTime FechaDetectada { get; set; } = DateTime.Now;

        public ICollection<EscaneosVulnerabilidades> EscaneosVulnerabilidades { get; set; }
        public ICollection<PlantillasVulnerabilidades> PlantillasVulnerabilidades { get; set; }
    }
}
