using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AuditSentinel;
using Xunit;

namespace AuditSentinel.Test
{
    public class ReportesTests
    {
        private static (bool valido, List<ValidationResult> results) Validar(object model)
        {
            var ctx     = new ValidationContext(model);
            var results = new List<ValidationResult>();
            var valido  = Validator.TryValidateObject(model, ctx, results, validateAllProperties: true);
            return (valido, results);
        }

        // ================================================================
        // NOMBRE DEL REPORTE
        // ================================================================

        /// Verifica que el nombre es obligatorio
        [Fact]
        public void Reporte_NombreReporte_Requerido()
        {
            var model = new Reportes
            {
                NombreReporte = "",
                cumplimiento  = Cumplimiento.Cumple
            };

            var (valido, results) = Validar(model);

            Assert.False(valido);
            Assert.Contains(results, r => r.MemberNames.Contains(nameof(Reportes.NombreReporte)));
        }

        /// Verifica longitud mínima (menos de 5 caracteres falla)
        /// y longitud máxima (más de 80 caracteres falla)
        [Theory]
        [InlineData("AB",    false)]   // 2 caracteres — muy corto (< 5)
        [InlineData("Info",  false)]   // 4 caracteres — justo por debajo del mínimo
        [InlineData("Audit", true)]    // 5 caracteres — exactamente el mínimo
        [InlineData("Reporte de Seguridad 2025", true)]   // normal
        [InlineData("Análisis de Cumplimiento ISO 27001", true)]  // con tilde
        public void Reporte_NombreReporte_Longitud(string nombre, bool esperado)
        {
            var model = new Reportes
            {
                NombreReporte = nombre,
                cumplimiento  = Cumplimiento.Cumple
            };

            var (valido, _) = Validar(model);
            Assert.Equal(esperado, valido);
        }

        /// Verifica que el nombre de 80 caracteres exactos es válido
        /// y 81 caracteres es inválido
        [Fact]
        public void Reporte_NombreReporte_LongitudMaxima_80_Valido()
        {
            var model = new Reportes
            {
                NombreReporte = new string('A', 80),
                cumplimiento  = Cumplimiento.Cumple
            };
            var (valido, _) = Validar(model);
            Assert.True(valido);
        }

        [Fact]
        public void Reporte_NombreReporte_LongitudMaxima_81_Invalido()
        {
            var model = new Reportes
            {
                NombreReporte = new string('A', 81),
                cumplimiento  = Cumplimiento.Cumple
            };
            var (valido, _) = Validar(model);
            Assert.False(valido);
        }

        /// Verifica la expresión regular: no permite caracteres especiales ni solo números
        [Theory]
        [InlineData("Reporte 2025",          true)]   // letras y números OK
        [InlineData("Análisis Técnico",      true)]   // tildes OK
        [InlineData("Cumplimiento GDPR",     true)]   // mayúsculas OK
        [InlineData("Reporte@2025",          false)]  // @ no permitido
        [InlineData("Informe#Crítico",       false)]  // # no permitido
        [InlineData("12345",                 false)]  // solo números no permitido
        [InlineData("Reporte_Interno",       false)]  // guion bajo no permitido
        [InlineData("Seguridad/Redes",       false)]  // slash no permitido
        public void Reporte_NombreReporte_RegularExpression(string nombre, bool esperado)
        {
            var model = new Reportes
            {
                NombreReporte = nombre,
                cumplimiento  = Cumplimiento.Cumple
            };

            var (valido, _) = Validar(model);
            Assert.Equal(esperado, valido);
        }

        // ================================================================
        //  cumplimiento — enum con 3 valores válidos 
        // ================================================================

        /// Verifica que los 3 valores del enum son aceptados
        [Theory]
        [InlineData(Cumplimiento.Cumple,             true)]
        [InlineData(Cumplimiento.NoCumple,           true)]
        [InlineData(Cumplimiento.ParcialmenteCumple, true)]
        public void Reporte_Cumplimiento_ValoresValidos(Cumplimiento cumplimiento, bool esperado)
        {
            var model = new Reportes
            {
                NombreReporte = "Reporte Válido",
                cumplimiento  = cumplimiento
            };

            var (valido, _) = Validar(model);
            Assert.Equal(esperado, valido);
        }

        // ================================================================
        //  FECHA DE CREACIÓN 
        // ================================================================

        /// Verifica que al crear un reporte la fecha se asigna automáticamente
        [Fact]
        public void Reporte_Creado_TieneValorPorDefecto()
        {
            var antes = DateTime.Now.AddSeconds(-1);
            var model = new Reportes
            {
                NombreReporte = "Reporte Test",
                cumplimiento  = Cumplimiento.Cumple
            };
            var despues = DateTime.Now.AddSeconds(1);

            Assert.True(model.Creado >= antes && model.Creado <= despues,
                $"Se esperaba fecha entre {antes} y {despues}, pero fue {model.Creado}");
        }

        /// Verifica que la fecha puede asignarse manualmente
        [Fact]
        public void Reporte_Creado_PuedeAsignarseFechaManual()
        {
            var fechaEsperada = new DateTime(2025, 1, 15, 10, 30, 0);
            var model = new Reportes
            {
                NombreReporte = "Reporte Histórico",
                cumplimiento  = Cumplimiento.NoCumple,
                Creado        = fechaEsperada
            };

            Assert.Equal(fechaEsperada, model.Creado);
        }

        // ================================================================
        // 4. MODELO COMPLETO — prueba integral
        // ================================================================

        /// Reporte completamente válido debe pasar todas las validaciones
        [Fact]
        public void Reporte_ModeloCompleto_Valido()
        {
            var model = new Reportes
            {
                NombreReporte = "Análisis de Seguridad 2025",
                cumplimiento  = Cumplimiento.ParcialmenteCumple,
                Creado        = DateTime.Now
            };

            var (valido, results) = Validar(model);

            Assert.True(valido, $"Se esperaba válido pero falló: {string.Join(", ", results.Select(r => r.ErrorMessage))}");
        }

        /// Reporte con nombre inválido y enum válido debe fallar
        [Fact]
        public void Reporte_ModeloCompleto_Invalido_NombreVacio()
        {
            var model = new Reportes
            {
                NombreReporte = "",
                cumplimiento  = Cumplimiento.Cumple,
                Creado        = DateTime.Now
            };

            var (valido, _) = Validar(model);
            Assert.False(valido);
        }
    }
}