using System.ComponentModel.DataAnnotations;

namespace AuditSentinel.Models
{
    public class EscaneosPlantillas
    {
        public int IdEscaneo { get; set; }
        public Escaneos Escaneos { get; set; }

        public int IdPlantilla { get; set; }
        public Plantillas Plantillas { get; set; }

    }
}
