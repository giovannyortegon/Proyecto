using System.ComponentModel.DataAnnotations;

namespace AuditSentinel.Models
{
    public class Login
    {
        // Username property with validation
        [Required(ErrorMessage = "El correo electronico es obligatorio")]
        [EmailAddress(ErrorMessage = "Debe ingresar correo electronico valido")]
        [Display(Name = "Correo Electronico")]
        public string Email { get; set; }

        // Password property with validation
        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        [Display(Name = "Recordar sesion")]
        public bool RememberMe { get; set; }
    }
}
