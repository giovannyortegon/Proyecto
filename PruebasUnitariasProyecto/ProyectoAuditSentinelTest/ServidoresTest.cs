using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProyectoAuditSentinel;
using Xunit;
using FluentAssertions;
using System.ComponentModel.DataAnnotations;

namespace ProyectoAuditSentinelTest
{
    public class ServidoresTest 
    {
        private static(bool valido, List<ValidationResult> results) Validar(object model)
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
        [InlineData(" Servidor1",false)]
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
        public void Servidores_Should_Have_Required_Properties()
        {
            // Arrange
            var servidores = new Servidores
            {
                IdServidor = 1,
                NombreServidor = "SDC-SWE2",
                IP = "192.168.1.10",
                SistemaOperativo = SistemaOperativo.WindowsServer2012,
                Create_is = DateTime.UtcNow
            };

            // Act & Assert
            servidores.Should().NotBeNull();
            servidores.IdServidor.Should().BeGreaterThan(0);
            servidores.NombreServidor.Should().NotBeNullOrEmpty();
            servidores.IP.Should().NotBeNullOrEmpty();
            servidores.SistemaOperativo.Should().Be(SistemaOperativo.WindowsServer2012);
            servidores.Create_is.Should().BeOnOrAfter(DateTime.MinValue);
         }

        [Fact]
        public void Servidores_Should_Allow_Valid_IP_Address()
        {
            // Arrange
            var servidor = new Servidores
            {
                IdServidor = 2,
                NombreServidor = "Servidor01",
                IP = "192.168.0.1",
                SistemaOperativo = SistemaOperativo.WindowsServer2012,
                Create_is = DateTime.UtcNow
            };

            // Act
            var ip = servidor.IP;

            // Assert
            ip.Should().Contain(".");
            ip.Should().NotBeNullOrEmpty();
        }
        [Fact]
        public void Servidores_Should_Have_Server_Name()
        {
            // Arrange
            var servidor = new Servidores
            {
                IdServidor = 3,
                NombreServidor = "ServidorProduccion",
                IP = "10.0.0.5",
                SistemaOperativo = SistemaOperativo.WindowsServer2012,
                Create_is = DateTime.UtcNow
            };

            // Act
            var nombre = servidor.NombreServidor;

            // Assert
            nombre.Should().NotBeNullOrWhiteSpace();
            nombre.Length.Should().BeGreaterThan(3);
        }
    }
}
