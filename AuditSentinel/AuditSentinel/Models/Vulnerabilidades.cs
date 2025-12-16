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
        [Display(Name = "Nombre")]
        public string NombreVulnerabilidad { get; set; }
        [Display(Name = "Riesgo")]
        public NivelRiesgo NivelRiesgo { get; set; }
        [MaxLength(200)]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }
        [MaxLength(500)]
        [Display(Name = "Comando")]
        public string Comando { get; set; }
        [MaxLength(200)]
        [Display(Name = "Resultado Esperado")]
        public string ResultadoEsperado { get; set; }
        [Display(Name = "Fecha Dectección")]
        public DateTime FechaDetectada { get; set; } = DateTime.Now;

        public ICollection<EscaneosVulnerabilidades> EscaneosVulnerabilidades { get; set; }
        public ICollection<PlantillasVulnerabilidades> PlantillasVulnerabilidades { get; set; }
    }
}
