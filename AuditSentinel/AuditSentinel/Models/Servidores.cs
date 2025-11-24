using System.ComponentModel.DataAnnotations;

namespace AuditSentinel.Models
{
    public class Servidores
    {
        [Key]
        public int IdServidor { get; set; }
        [Required]
        [MaxLength(50)]
        public string NombreServidor { get; set; }
        [Required]
        [MaxLength(30)]
        public string IP { get; set; }
        [Required]
        [MaxLength(100)]
        public string SistemaOperativo { get; set; }
        public DateTime Create_is { get; set; } = DateTime.Now;

        public ICollection<EscaneosServidores> EscaneosServidores { get; set; }

    }
}
