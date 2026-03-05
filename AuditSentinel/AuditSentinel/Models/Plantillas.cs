using System.ComponentModel.DataAnnotations;

namespace AuditSentinel.Models
{
    public class Plantillas
    {
        [Key]
        public int IdPlantilla { get; set; }
        [Required]
        [MaxLength(80)]
        [StringLength(80, MinimumLength = 5, ErrorMessage = "El nombre debe tener entre 5 y 80 caracteres.")]
        [RegularExpression(@"^(?!\d+$)[a-zA-Z0-9áéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "El nombre no puede tener caracteres especiales o ser solo numeros. ")]
        public string NombrePlantilla { get; set; }
        [Required]
        [MaxLength(80)]
        [StringLength(80, MinimumLength = 1, ErrorMessage = "El nombre debe tener entre 5 y 80 caracteres.")]
        public string Version { get; set; }

        public ICollection<EscaneosPlantillas> EscaneosPlantillas { get; set; }
        public ICollection<PlantillasVulnerabilidades> PlantillasVulnerabilidades { get; set; }
    }
}
