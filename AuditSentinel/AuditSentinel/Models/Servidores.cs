using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AuditSentinel.Models
{
    public enum SistemaOperativo
    {

        [Display(Name = "Linux")]
        Linux,

        [Display(Name = "Windows Server 2012")]
        WindowsServer2012,

        [Display(Name = "Windows Server 2016")]
        WindowsServer2016,

        [Display(Name = "Windows Server 2019")]
        WindowsServer2019,

        [Display(Name = "Windows Server 2022")]
        WindowsServer2022,

        [Display(Name = "Windows Server 2025")]
        WindowsServer2025

    }
    public class Servidores
    {
        [Key]
        public int IdServidor { get; set; }
        [Required]
        [MaxLength(80)]
        [StringLength(80, MinimumLength = 5, ErrorMessage = "El nombre debe tener entre 5 y 80 caracteres.")]
        [RegularExpression(@"^(?!\d+$)[a-zA-Z0-9áéíóúÁÉÍÓÚñÑ]+$",
            ErrorMessage = "El nombre no puede tener caracteres especiales, contener espacios o ser solo números.")]
        [Display(Name = "Nombre Servidor")]
        public string NombreServidor { get; set; }
        [Required]
        [MaxLength(30)]
        //[StringLength(16, MinimumLength = 16, ErrorMessage = "La IP debe tener 15 caracteres.") ]
        [RegularExpression(@"^(?:\d{1,3}\.){3}\d{1,3}$", ErrorMessage = "La IP debe tener el formato correcto (ej. 192.168.0.1).")]
        [Display(Name = "Direccion IP")]
        public string IP { get; set; }

        [Required(ErrorMessage = "Seleccione un sistema operativo.")]
        [Display(Name = "Sistema Operativo")]
        public SistemaOperativo? SistemaOperativo { get; set; }  // nullable + [Required]


        [Display(Name = "Fecha Creacion")]
        public DateTime Create_is { get; set; } = DateTime.Now;

        public ICollection<EscaneosServidores> EscaneosServidores { get; set; }


    }
}
