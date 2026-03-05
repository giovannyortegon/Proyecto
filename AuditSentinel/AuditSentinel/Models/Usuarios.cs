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
        [RegularExpression(@"^(?!\d+$)[a-zA-Z0-9áéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "El nombre de usuario no puede tener caracteres especiales o ser solo numeros. ")]
        public string Nombre { get; set; }

        [Required]
        [Display(Name = "Apellido")]
        [MaxLength(80)]
        [StringLength(80, MinimumLength = 5, ErrorMessage = "El apellido debe tener entre 5 y 80 caracteres.")]
        [RegularExpression(@"^(?!\d+$)[a-zA-Z0-9áéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "El apellido no puede tener caracteres especiales o ser solo numeros. ")]
        public string Apellido { get; set; }

        [Display(Name = "Fecha Creacion")]
        public DateTime FechaCreado { get; set; } = DateTime.Now;
    }
}
