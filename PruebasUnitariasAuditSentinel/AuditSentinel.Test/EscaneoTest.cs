using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using AuditSentinel;
using Xunit;

namespace AuditSentinel.Test
{
    public class EscaneosTests
    {
        // ================================================================
        // HELPER: ejecuta las DataAnnotations sobre cualquier modelo
        // ================================================================
        private static (bool valido, List<ValidationResult> results) Validar(object model)
        {
            var ctx     = new ValidationContext(model);
            var results = new List<ValidationResult>();
            var valido  = Validator.TryValidateObject(model, ctx, results, validateAllProperties: true);
            return (valido, results);
        }

        // ================================================================
        // 1. NOMBRE DEL ESCANEO — [Required] + [StringLength(5,80)] + [RegularExpression]
        // ================================================================

        /// Verifica que el nombre es obligatorio
        [Fact]
        public void Escaneo_NombreEscaneo_Requerido()
        {
            var model = new Escaneos
            {
                NombreEscaneo = "",
                Estado        = EstadoEscaneo.Pendiente
            };

            var (valido, results) = Validar(model);

            Assert.False(valido);
            Assert.Contains(results, r => r.MemberNames.Contains(nameof(Escaneos.NombreEscaneo)));
        }

        /// Verifica longitud mínima y máxima
        [Theory]
        [InlineData("AB",    false)]   // 2 chars — muy corto
        [InlineData("Scan",  false)]   // 4 chars — justo por debajo del mínimo
        [InlineData("Audit", true)]    // 5 chars — exactamente el mínimo
        [InlineData("Escaneo de Red Interna 2025", true)]   // normal
        [InlineData("Análisis de Vulnerabilidades Servidor", true)]  // con tilde
        public void Escaneo_NombreEscaneo_Longitud(string nombre, bool esperado)
        {
            var model = new Escaneos
            {
                NombreEscaneo = nombre,
                Estado        = EstadoEscaneo.Pendiente
            };

            var (valido, _) = Validar(model);
            Assert.Equal(esperado, valido);
        }

        /// 80 caracteres exactos — válido
        [Fact]
        public void Escaneo_NombreEscaneo_LongitudMaxima_80_Valido()
        {
            var model = new Escaneos
            {
                NombreEscaneo = new string('A', 80),
                Estado        = EstadoEscaneo.Pendiente
            };
            var (valido, _) = Validar(model);
            Assert.True(valido);
        }

        /// 81 caracteres — inválido
        [Fact]
        public void Escaneo_NombreEscaneo_LongitudMaxima_81_Invalido()
        {
            var model = new Escaneos
            {
                NombreEscaneo = new string('A', 81),
                Estado        = EstadoEscaneo.Pendiente
            };
            var (valido, _) = Validar(model);
            Assert.False(valido);
        }

        /// Verifica la expresión regular
        [Theory]
        [InlineData("Escaneo 2025",          true)]   // letras y números OK
        [InlineData("Análisis Técnico",      true)]   // tildes OK
        [InlineData("Scan RED Interna",      true)]   // mayúsculas OK
        [InlineData("Escaneo@Red",           false)]  // @ no permitido
        [InlineData("Scan#Crítico",          false)]  // # no permitido
        [InlineData("12345",                 false)]  // solo números no permitido
        [InlineData("Escaneo_Interno",       false)]  // guion bajo no permitido
        [InlineData("Red/Local",             false)]  // slash no permitido
        [InlineData("Scan-Externo",          false)]  // guion no permitido
        public void Escaneo_NombreEscaneo_RegularExpression(string nombre, bool esperado)
        {
            var model = new Escaneos
            {
                NombreEscaneo = nombre,
                Estado        = EstadoEscaneo.Pendiente
            };

            var (valido, _) = Validar(model);
            Assert.Equal(esperado, valido);
        }

        // ================================================================
        // 2. ESTADO — enum con 4 valores válidos
        // ================================================================

        [Theory]
        [InlineData(EstadoEscaneo.Pendiente,   true)]
        [InlineData(EstadoEscaneo.EnProgreso,  true)]
        [InlineData(EstadoEscaneo.Completado,  true)]
        [InlineData(EstadoEscaneo.Fallido,     true)]
        public void Escaneo_Estado_TodosLosValoresValidos(EstadoEscaneo estado, bool esperado)
        {
            var model = new Escaneos
            {
                NombreEscaneo = "Escaneo Válido",
                Estado        = estado
            };

            var (valido, _) = Validar(model);
            Assert.Equal(esperado, valido);
        }

        /// Verifica que el estado por defecto al instanciar es Pendiente (valor 0 del enum)
        [Fact]
        public void Escaneo_Estado_ValorPorDefectoEsPendiente()
        {
            var model = new Escaneos
            {
                NombreEscaneo = "Escaneo Nuevo"
            };

            Assert.Equal(EstadoEscaneo.Pendiente, model.Estado);
        }

        // ================================================================
        // 3. TRANSICIONES DE ESTADO — lógica de flujo
        // ================================================================

        /// Verifica la secuencia lógica de estados
        [Fact]
        public void Escaneo_Estado_TransicionPendienteAEnProgreso()
        {
            var model = new Escaneos
            {
                NombreEscaneo = "Escaneo Test",
                Estado        = EstadoEscaneo.Pendiente
            };

            model.Estado = EstadoEscaneo.EnProgreso;

            Assert.Equal(EstadoEscaneo.EnProgreso, model.Estado);
        }

        [Fact]
        public void Escaneo_Estado_TransicionEnProgresoACompletado()
        {
            var model = new Escaneos
            {
                NombreEscaneo = "Escaneo Test",
                Estado        = EstadoEscaneo.EnProgreso
            };

            model.Estado = EstadoEscaneo.Completado;

            Assert.Equal(EstadoEscaneo.Completado, model.Estado);
        }

        [Fact]
        public void Escaneo_Estado_TransicionEnProgresoAFallido()
        {
            var model = new Escaneos
            {
                NombreEscaneo = "Escaneo Test",
                Estado        = EstadoEscaneo.EnProgreso
            };

            model.Estado = EstadoEscaneo.Fallido;

            Assert.Equal(EstadoEscaneo.Fallido, model.Estado);
        }

        // ================================================================
        // 4. FECHA DE ESCANEO — valor por defecto DateTime.Now
        // ================================================================

        [Fact]
        public void Escaneo_FechaEscaneo_TieneValorPorDefecto()
        {
            var antes  = DateTime.Now.AddSeconds(-1);
            var model  = new Escaneos { NombreEscaneo = "Escaneo Test" };
            var despues = DateTime.Now.AddSeconds(1);

            Assert.True(model.FechaEscaneo >= antes && model.FechaEscaneo <= despues,
                $"Se esperaba fecha entre {antes} y {despues}, pero fue {model.FechaEscaneo}");
        }

        [Fact]
        public void Escaneo_FechaEscaneo_PuedeAsignarseFechaManual()
        {
            var fechaEsperada = new DateTime(2025, 6, 15, 9, 0, 0);
            var model = new Escaneos
            {
                NombreEscaneo = "Escaneo Histórico",
                Estado        = EstadoEscaneo.Completado,
                FechaEscaneo  = fechaEsperada
            };

            Assert.Equal(fechaEsperada, model.FechaEscaneo);
        }

        // ================================================================
        // 6. MODELO COMPLETO — prueba integral
        // ================================================================

        [Fact]
        public void Escaneo_ModeloCompleto_Valido()
        {
            var model = new Escaneos
            {
                NombreEscaneo = "Escaneo de Red 2025",
                Estado        = EstadoEscaneo.Completado,
                FechaEscaneo  = DateTime.Now
            };

            var (valido, results) = Validar(model);

            Assert.True(valido,
                $"Se esperaba válido pero falló: {string.Join(", ", results.Select(r => r.ErrorMessage))}");
        }

        [Fact]
        public void Escaneo_ModeloCompleto_Invalido_NombreVacio()
        {
            var model = new Escaneos
            {
                NombreEscaneo = "",
                Estado        = EstadoEscaneo.Pendiente
            };

            var (valido, _) = Validar(model);
            Assert.False(valido);
        }

        [Fact]
        public void Escaneo_ModeloCompleto_Invalido_NombreSoloNumeros()
        {
            var model = new Escaneos
            {
                NombreEscaneo = "99999",
                Estado        = EstadoEscaneo.Pendiente
            };

            var (valido, results) = Validar(model);

            Assert.False(valido);
            Assert.Contains(results, r => r.ErrorMessage.Contains("caracteres especiales"));
        }
    }
}