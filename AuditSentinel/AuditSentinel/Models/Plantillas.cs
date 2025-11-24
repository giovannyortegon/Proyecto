using System.ComponentModel.DataAnnotations;

namespace AuditSentinel.Models
{
    public class Plantillas
    {
        [Key]
        public int IdPlantilla { get; set; }
        [Required]
        [MaxLength(100)]
        public string NombrePlantilla { get; set; }
        [Required]
        [MaxLength(20)]
        public string Version { get; set; }

        public ICollection<EscaneosPlantillas> EscaneosPlantillas { get; set; }
        public ICollection<PlantillasVulnerabilidades> PlantillasVulnerabilidades { get; set; }
    }
}
