using System.ComponentModel.DataAnnotations;

namespace AuditSentinel.Models
{
    public class Servidores
    {
        [Key]
        public int IdServidor { get; set; }
        [Required]
        [MaxLength(80)]
        [StringLength(80, MinimumLength = 5, ErrorMessage = "El nombre debe tener entre 5 y 80 caracteres.")]
        [RegularExpression(@"^(?!\d+$).+", ErrorMessage = "El nombre de usuario no puede ser solo números.")]
        [Display(Name = "Nombre Servidor")]
        public string NombreServidor { get; set; }
        [Required]
        [MaxLength(30)]
        [StringLength(16, MinimumLength = 16, ErrorMessage = "La IP debe tener 15 caracteres.") ]
        [Display(Name = "Direccion IP")]
        public string IP { get; set; }
        [Required]
        [MaxLength(80)]
        [StringLength(80, MinimumLength = 5, ErrorMessage = "El nombre debe tener entre 5 y 80 caracteres.")]
        [RegularExpression(@"^(?!\d+$).+", ErrorMessage = "El nombre de usuario no puede ser solo números.")]
        [Display(Name = "Sistema Operativo")]
        public string SistemaOperativo { get; set; }
        [Display(Name = "Fecha Creacion")]
        public DateTime Create_is { get; set; } = DateTime.Now;

        public ICollection<EscaneosServidores> EscaneosServidores { get; set; }

    }
}
