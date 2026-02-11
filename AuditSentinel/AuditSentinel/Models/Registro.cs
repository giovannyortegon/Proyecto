using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace AuditSentinel.Models
{
    public class Registro
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatoria")]
        [StringLength(100, ErrorMessage = "El nombre no debe exceder 100 caracteres")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El nombre es obligatoria")]
        [StringLength(100, ErrorMessage = "El nombre no debe exceder 100 caracteres")]
        [Display(Name = "Nombre")]
        public string Apellido { get; set; }

        public string UserName { get; set; }

        // Username property with validation
        [Required(ErrorMessage = "El correo electronico es obligatorio")]
       //[EmailAddress(ErrorMessage = "El correo electronico no es valido")]
        [Display(Name = "Correo Electronico")]
        public string Email { get; set; }

        // Password property with validation
        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }


        [Required(ErrorMessage = "Debe confirmar contraseña")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
        [Display(Name = "Confirmar Contraseña")]
        public string ConfirmacionPassword { get; set; }

        [Required(ErrorMessage = "El nombre es obligatoria")]
        [Display(Name = "Rol")]
        public List<string> Rol { get; set; }

        [Display(Name = "Fecha Creacion")]
        public DateTime FechaCreado { get; set; } = DateTime.Now;
    }
}
