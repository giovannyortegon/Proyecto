using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProyectoAuditSentinel;
using Xunit;
using System.ComponentModel.DataAnnotations;

namespace AuditSentinel.Test
{
    public class ServidoresTest
    {
        private static (bool valido, List<ValidationResult> results) Validar(object model)
        {
            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();
            var valido = Validator.TryValidateObject(model, context, results, true);
            return (valido, results);
        }
        private readonly Servidores _servidores = new Servidores();
        [Theory]
        [InlineData("Servidor1", true)]
        [InlineData("", false)]
        [InlineData(null, false)]
        [InlineData("   ", false)]
        [InlineData(" Servidor1", false)]
        [InlineData("Servidor1 ", false)]
        [InlineData(" Servidor1 ", false)]
        [InlineData("Servidor 1", false)]
        [InlineData("Servidor-1", true)]
        [InlineData("Servidor_1", true)]
        [InlineData("Servidor.1", false)]
        [InlineData("Servidor@1", false)]
        [InlineData("1111111111", false)]
        [InlineData("Serv", false)]
        [InlineData("ServidorConUnNombreMuyLargoQueExcedeElLimitePermitido", false)]
        [InlineData("Servidór", false)]
        public void RegistroNombreObligatorio(string nombre, bool ValidadorEsperado)
        {
            // Arrange
            var model = new Servidores
            {
                IdServidor = 1,
                NombreServidor = nombre, // NombreServidor es obligatorio
                IP = "192.168.1.10",
                SistemaOperativo = SistemaOperativo.WindowsServer2012,
                Create_is = DateTime.UtcNow
            };

            var (valido, _) = Validar(model);
            Assert.Equal(ValidadorEsperado, valido);
        }



        [Theory]
        [InlineData("192.168.1.12", true)]
        [InlineData("192.168.1.1a", false)]
        [InlineData("1a2.168.1.11", false)]
        [InlineData("112.168.a.11", false)]
        [InlineData("192.16a.1.11", false)]
        [InlineData("a92.16a.a.1a", false)]
        [InlineData("", false)]
        [InlineData(null, false)]
        [InlineData("8324832", false)]
        [InlineData("1111..111", false)]
        public void ServidoresIpTest(string Ip, bool ValidadorEsperado)
        {
            // Arrange
            var model = new Servidores
            {
                IdServidor = 2,
                NombreServidor = "Servidor01",
                IP = Ip,
                SistemaOperativo = SistemaOperativo.WindowsServer2012,
                Create_is = DateTime.UtcNow
            };
            var (valido, _) = Validar(model);
            Assert.Equal(ValidadorEsperado, valido);
        }

    }
}
