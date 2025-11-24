using System.ComponentModel.DataAnnotations;

namespace AuditSentinel.Models
{
    public enum TipoRol
    {

    }
    public class Roles
    {
        [Key]
        public int IdRole { get; set; }
        public string RoleName { get; set; }
    }
}
