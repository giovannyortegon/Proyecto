using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProyectoAuditSentinel;
using Xunit;
using FluentAssertions;

namespace ProyectoAuditSentinelTest
{
    public class ServidoresTest 
    {
        private readonly Servidores _servidores = new Servidores();
        [Fact]

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
    }
}
