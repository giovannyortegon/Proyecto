using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AuditSentinel.Models
{
    public class Correo
    {
        [Key]
        public int EmailId { get; set; }
        public string Nombre { get; set; }
        public string Empresa { get; set; }
        [Required(ErrorMessage = "El correo electrónico es obligatorio")]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        ErrorMessage = "Debe ingresar un correo electrónico válido")]
        public string Email { get; set; }
        public string Mensaje { get; set; }
    }
}
