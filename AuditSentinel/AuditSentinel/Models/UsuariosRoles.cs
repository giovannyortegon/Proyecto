namespace AuditSentinel.Models
{
    public class UsuariosRoles
    {
        public int IdUsuario { get; set; }
        public Usuarios usuarios { get; set; }
        public int IdRol { get; set; }
        public Roles roles { get; set; }
    }
}
