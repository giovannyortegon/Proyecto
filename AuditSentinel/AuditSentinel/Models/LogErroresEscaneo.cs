using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuditSentinel.Models
{
    public class LogErroresEscaneo
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int EscaneoId { get; set; }

        [ForeignKey("EscaneoId")]
        public virtual Escaneos Escaneo { get; set; }

        public DateTime FechaError { get; set; } = DateTime.Now;

        // Ej: "Conexión", "Permisos Nmap", "Timeout"
        public string Fase { get; set; }

        // El mensaje de excepción o el "StandardError" de Nmap
        public string Mensaje { get; set; }

        // El comando exacto que causó el problema para replicarlo
        public string ComandoEjecutado { get; set; }  
    }
}
