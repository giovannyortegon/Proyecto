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
        [MaxLength(80)]
        [StringLength(80, MinimumLength = 5, ErrorMessage = "El nombre debe tener entre 5 y 80 caracteres.")]
        [RegularExpression(@"^(?!\d+$)[a-zA-Z0-9áéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "El nombre de usuario no puede tener caracteres especiales o ser solo numeros. ")]
        [Display(Name = "Nombre Escaneo")]
        public string NombreEscaneo { get; set; }

        [Display(Name = "Estado")]
        public EstadoEscaneo Estado { get; set; }   
        [Display(Name = "Fecha Escaneo")]
        public DateTime FechaEscaneo { get; set; } = DateTime.Now;

        public ICollection<EscaneosServidores> EscaneosServidores { get; set; }
        public ICollection<EscaneosPlantillas> EscaneosPlantillas { get; set; }
        public ICollection<EscaneosReportes> EscaneosReportes { get; set; }
        public ICollection<EscaneosVulnerabilidades> EscaneosVulnerabilidades { get; set; }
        public virtual ICollection<LogErroresEscaneo> Logs { get; set; } = new List<LogErroresEscaneo>();
    }
}
