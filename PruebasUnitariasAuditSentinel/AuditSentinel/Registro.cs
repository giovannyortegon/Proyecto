using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuditSentinel
{
    public class Registro
    {
        public string Id { get; set; }


        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no debe exceder 100 caracteres")]
        [MinLength(4, ErrorMessage = "El nombre debe tener al menos 4 caracteres")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ]+(?:\s[a-zA-ZáéíóúÁÉÍÓÚñÑ]+)*$",
            ErrorMessage = "El nombre no debe tener caracteres especiales ni números.")]
        [Display(Name = "Nombre")]

        public string Nombre { get; set; }

        [Required(ErrorMessage = "El apellido es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no debe exceder 100 caracteres")]
        [MinLength(4, ErrorMessage = "El nombre debe tener al menos 4 caracteres")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ]+(?:\s[a-zA-ZáéíóúÁÉÍÓÚñÑ]+)*$",
            ErrorMessage = "El nombre no debe tener caracteres especiales ni números.")]
        [Display(Name = "Apelido")]
        public string Apellido { get; set; }

        public string UserName { get; set; }

        // Username property with validation
        [Required(ErrorMessage = "El correo electronico es obligatorio")]
        [EmailAddress(ErrorMessage = "El correo electronico no es valido")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Formato de correo inválido.")]
        [Display(Name = "Correo Electronico")]
        public string Email { get; set; }

        // Password property with validation
        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[^\w\s]).{8,}$",
ErrorMessage = "Debe tener al menos 8 caracteres, con mayúsculas, minúsculas, números y un carácter especial.")]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }


        [Required(ErrorMessage = "Debe confirmar contraseña")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Las contraseñas no coincide")]
        [Display(Name = "Confirmar Contraseña")]
        public string ConfirmacionPassword { get; set; }

        [Required(ErrorMessage = "El rol es obligatoria")]
        [Display(Name = "Rol")]
        public List<string> Rol { get; set; }

        [Display(Name = "Fecha Creacion")]
        public DateTime FechaCreado { get; set; } = DateTime.Now;
    }
}
