using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace AuditSentinel.Models
{
    public class Usuarios 
    {
    
        [Required]
        [Display(Name = "Nombre Usuario")]
        [MaxLength(80)]
        [StringLength(80, MinimumLength = 5, ErrorMessage = "El nombre debe tener entre 5 y 80 caracteres.")]
        [RegularExpression(@"^(?!\d+$).+", ErrorMessage = "El nombre de usuario no puede ser solo números.")]
        public string NombreUsiario { get; set; }
        [Display(Name = "Contraseña")]
        public string Password { get; set; }
        [Required]
        [Display(Name = "Correo")]
        [MaxLength(80)]
        [StringLength(80, MinimumLength = 5, ErrorMessage = "El correo debe tener entre 5 y 80 caracteres.")]
        [RegularExpression(@"^(?!\d+$).+", ErrorMessage = "El correo no puede ser solo números.")]
        public string Correo { get; set; }
        [Display(Name = "Fecha Creacion")]
        public DateTime FechaCreado { get; set; } = DateTime.Now;

  
    }
}
