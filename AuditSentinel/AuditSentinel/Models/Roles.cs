using System.ComponentModel.DataAnnotations;

namespace AuditSentinel.Models
{
    public enum TipoRol
    {
        Administrador,
        Analistta,
        Auditor
    }
    public class Roles
    {
        [Key]
        public int IdRole { get; set; }
        public TipoRol NombreRol { get; set; }

        public ICollection<UsuariosRoles> UsuariosRoles { get; set; }
    }
}
