namespace AuditSentinel.Models
{
    public class Usuarios
    {
        public int IdUsuario { get; set; }
        public string nombreUsuario { get; set; }
        public string contresena { get; set; }
        public string correo { get; set; }
        public DateTime FechaEscaneo { get; set; } = DateTime.Now;

        public ICollection<UsuariosRoles> UsuariosRoles { get; set; }
    }
}
