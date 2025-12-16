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
        //[StringLength(16, MinimumLength = 16, ErrorMessage = "La IP debe tener 15 caracteres.") ]
        [RegularExpression(@"^(?:\d{1,3}\.){3}\d{1,3}$", ErrorMessage = "La IP debe tener el formato correcto (ej. 192.168.0.1).")]
        [Display(Name = "Direccion IP")]
        public string IP { get; set; }
        [Required]
        [MaxLength(80)]
        [StringLength(80, MinimumLength = 5, ErrorMessage = "El nombre debe tener entre 5 y 80 caracteres.")]
        [RegularExpression(@"^(Linux( Suse)?|RedHat( 8)?|Windows Server (2012|2016|2019|2022|2025))$",
                            ErrorMessage = "Sistema operativo no permitido deber ser Linux o Windows Server (2012 entre 2025)."
        )]
        [Display(Name = "Sistema Operativo")]
        public string SistemaOperativo { get; set; }
        [Display(Name = "Fecha Creacion")]
        public DateTime Create_is { get; set; } = DateTime.Now;

        public ICollection<EscaneosServidores> EscaneosServidores { get; set; }

    }
}
