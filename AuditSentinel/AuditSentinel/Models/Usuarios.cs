using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace AuditSentinel.Models
{
    public class Usuarios : IdentityUser
    {
    
        [Required]
        [Display(Name = "Nombre")]
        [MaxLength(80)]
        [StringLength(80, MinimumLength = 5, ErrorMessage = "El nombre debe tener entre 5 y 80 caracteres.")]
        [RegularExpression(@"^(?!\d+$).+", ErrorMessage = "El nombre de usuario no puede ser solo números.")]
        public string Nombre { get; set; }

        [Required]
        [Display(Name = "Apellido")]
        [MaxLength(80)]
        [StringLength(80, MinimumLength = 5, ErrorMessage = "El apellido debe tener entre 5 y 80 caracteres.")]
        [RegularExpression(@"^(?!\d+$).+", ErrorMessage = "El apellido de usuario no puede ser solo números.")]
        public string Apellido { get; set; }

        [Display(Name = "Fecha Creacion")]
        public DateTime FechaCreado { get; set; } = DateTime.Now;
    }
}
