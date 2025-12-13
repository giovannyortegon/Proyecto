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
        [MaxLength(80)]
        [StringLength(80, MinimumLength = 5, ErrorMessage = "El nombre debe tener entre 5 y 80 caracteres.")]
        [RegularExpression(@"^(?!\d+$).+", ErrorMessage = "El nombre de usuario no puede ser solo números.")]
        [Display(Name = "Nombre Reporte")]
        public string NombreReporte { get; set; }
        [Display(Name = "Cumplimiento")]
        public Cumplimiento cumplimiento { get; set; }
        [Display(Name = "Fecha Creación")]
        public DateTime Creado { get; set; } = DateTime.Now;

        public ICollection<EscaneosReportes> EscaneosReportes { get; set; }
    }
}
