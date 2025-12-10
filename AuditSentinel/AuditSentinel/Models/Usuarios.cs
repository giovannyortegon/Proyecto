using System.ComponentModel.DataAnnotations;

namespace AuditSentinel.Models
{
    public class Usuarios
    {
        [Key]
        public int IdUsuario { get; set; }
        [Required]
        [MaxLength(80)]
        [StringLength(80, MinimumLength = 5, ErrorMessage = "El nombre debe tener entre 5 y 80 caracteres.")]
        [RegularExpression(@"^(?!\d+$).+", ErrorMessage = "El nombre de usuario no puede ser solo números.")]
        public string nombreUsuario { get; set; }
        public string contresena { get; set; }
        [Required]
        [MaxLength(80)]
        [StringLength(80, MinimumLength = 5, ErrorMessage = "El correo debe tener entre 5 y 80 caracteres.")]
        [RegularExpression(@"^(?!\d+$).+", ErrorMessage = "El correo no puede ser solo números.")]
        public string correo { get; set; }
        public DateTime FechaEscaneo { get; set; } = DateTime.Now;

        public ICollection<UsuariosRoles> UsuariosRoles { get; set; }
    }
}
