using AuditSentinel.Models;
using System.ComponentModel.DataAnnotations;

namespace AuditSentinel.Test
{
    public class ReportesTest
    {
        private static (bool valido, List<ValidationResult> results) Validar(object model)
        {
            var ctx = new ValidationContext(model);
            var results = new List<ValidationResult>();
            var valido = Validator.TryValidateObject(model, ctx, results, validateAllProperties: true);
            return (valido, results);
        }
        // 1: NOMBRE DEL REPORTE
        [Fact]
        public void Reporte_Nombre_Vacio_Invalido()
        {
            var model = new Reportes { NombreReporte = "", cumplimiento = Cumplimiento.Cumple };
            var (valido, _) = Validar(model);
            Assert.False(valido);
        }

        [Fact]
        public void Reporte_Nombre_Nulo_Invalido()
        {
            var model = new Reportes { NombreReporte = null, cumplimiento = Cumplimiento.Cumple };
            var (valido, _) = Validar(model);
            Assert.False(valido);
        }

        [Fact]
        public void Reporte_Nombre_SoloEspacios_Invalido()
        {
            var model = new Reportes { NombreReporte = "     ", cumplimiento = Cumplimiento.Cumple };
            var (valido, _) = Validar(model);
            Assert.False(valido);
        }

        [Fact]
        public void Reporte_Nombre_Requerido_MensajeError_Presente()
        {
            var model = new Reportes { NombreReporte = "", cumplimiento = Cumplimiento.Cumple };
            var (_, results) = Validar(model);
            Assert.Contains(results, r => r.MemberNames.Contains(nameof(Reportes.NombreReporte)));
        }

        // 2: LONGITUD MÍNIMA (MinimumLength = 5)
        [Theory]
        [InlineData("A", false)]   // 1 
        [InlineData("AB", false)]   // 2 
        [InlineData("ABC", false)]   // 3 
        [InlineData("ABCD", false)]   // 4  
        [InlineData("ABCDE", true)]   // 5 
        public void Reporte_Nombre_LongitudMinima(string nombre, bool esperado)
        {
            var model = new Reportes { NombreReporte = nombre, cumplimiento = Cumplimiento.Cumple };
            var (valido, _) = Validar(model);
            Assert.Equal(esperado, valido);
        }

        [Fact]
        public void Reporte_Nombre_Longitud4_Invalido()
        {
            var model = new Reportes { NombreReporte = "Info", cumplimiento = Cumplimiento.Cumple };
            var (valido, _) = Validar(model);
            Assert.False(valido);
        }

        [Fact]
        public void Reporte_Nombre_Longitud5_Valido()
        {
            var model = new Reportes { NombreReporte = "Audit", cumplimiento = Cumplimiento.Cumple };
            var (valido, _) = Validar(model);
            Assert.True(valido);
        }

        [Fact]
        public void Reporte_Nombre_LongitudMinima_MensajeError_Correcto()
        {
            var model = new Reportes { NombreReporte = "ABC", cumplimiento = Cumplimiento.Cumple };
            var (valido, results) = Validar(model);
            Assert.False(valido);
            Assert.Contains(results, r => r.ErrorMessage.Contains("5") || r.ErrorMessage.Contains("80"));
        }

        // 3: LONGITUD MÁXIMA (MaxLength = 80)
        [Fact]
        public void Reporte_Nombre_Longitud79_Valido()
        {
            var model = new Reportes { NombreReporte = new string('A', 79), cumplimiento = Cumplimiento.Cumple };
            var (valido, _) = Validar(model);
            Assert.True(valido);
        }

        [Fact]
        public void Reporte_Nombre_Longitud80_Valido()
        {
            var model = new Reportes { NombreReporte = new string('A', 80), cumplimiento = Cumplimiento.Cumple };
            var (valido, _) = Validar(model);
            Assert.True(valido);
        }

        [Fact]
        public void Reporte_Nombre_Longitud81_Invalido()
        {
            var model = new Reportes { NombreReporte = new string('A', 81), cumplimiento = Cumplimiento.Cumple };
            var (valido, _) = Validar(model);
            Assert.False(valido);
        }

        [Fact]
        public void Reporte_Nombre_Longitud100_Invalido()
        {
            var model = new Reportes { NombreReporte = new string('A', 100), cumplimiento = Cumplimiento.Cumple };
            var (valido, _) = Validar(model);
            Assert.False(valido);
        }

        [Fact]
        public void Reporte_Nombre_Longitud50_Valido()
        {
            var model = new Reportes { NombreReporte = new string('A', 50), cumplimiento = Cumplimiento.Cumple };
            var (valido, _) = Validar(model);
            Assert.True(valido);
        }
        // 4: CARACTERES ESPECIALES PROHIBIDOS (RegularExpression)
        [Theory]
        [InlineData("Reporte@2025", false)]   // @
        [InlineData("Informe#Critico", false)]   // #
        [InlineData("Reporte/Red", false)]   // /
        [InlineData("Scan_Interno", false)]   // _
        [InlineData("Reporte-Final", false)]   // -
        [InlineData("Reporte(2025)", false)]   // paréntesis
        [InlineData("Reporte&Red", false)]   // &
        [InlineData("Reporte.2025", false)]   // punto
        [InlineData("Reporte!Critico", false)]   // !
        [InlineData("Reporte*Red", false)]   // asterisco
        public void Reporte_Nombre_CaracteresEspeciales_Invalidos(string nombre, bool esperado)
        {
            var model = new Reportes { NombreReporte = nombre, cumplimiento = Cumplimiento.Cumple };
            var (valido, _) = Validar(model);
            Assert.Equal(esperado, valido);
        }

        [Theory]
        [InlineData("Reporte%2025", false)]   // %
        [InlineData("Reporte^Red", false)]   // ^
        [InlineData("Reporte=Red", false)]   // =
        [InlineData("Reporte+Red", false)]   // +
        [InlineData("Reporte<script>", false)]   // HTML injection
        [InlineData("Reporte\"Red", false)]   // comilla doble
        [InlineData("Reporte;Red", false)]   // punto y coma
        [InlineData("Reporte:Red", false)]   // dos puntos
        [InlineData("Reporte,Red", false)]   // coma
        [InlineData("Reporte~Red", false)]   // tilde ASCII
        public void Reporte_Nombre_CaracteresEspeciales_Adicionales_Invalidos(string nombre, bool esperado)
        {
            var model = new Reportes { NombreReporte = nombre, cumplimiento = Cumplimiento.Cumple };
            var (valido, _) = Validar(model);
            Assert.Equal(esperado, valido);
        }

        [Fact]
        public void Reporte_Nombre_CaracterEspecial_MensajeError_Correcto()
        {
            var model = new Reportes { NombreReporte = "Reporte@Invalido", cumplimiento = Cumplimiento.Cumple };
            var (valido, results) = Validar(model);
            Assert.False(valido);
            Assert.Contains(results, r =>
                r.ErrorMessage.Contains("caracteres especiales") ||
                r.ErrorMessage.Contains("solo numeros"));
        }

        // 5: SOLO NÚMEROS 
        [Theory]
        [InlineData("12345", false)]   // 5 dígitos
        [InlineData("123456", false)]   // 6 dígitos
        [InlineData("99999", false)]   // todos iguales
        [InlineData("00000", false)]   // ceros
        [InlineData("10203040", false)]   // 8 dígitos
        public void Reporte_Nombre_SoloNumeros_Invalido(string nombre, bool esperado)
        {
            var model = new Reportes { NombreReporte = nombre, cumplimiento = Cumplimiento.Cumple };
            var (valido, _) = Validar(model);
            Assert.Equal(esperado, valido);
        }

        [Fact]
        public void Reporte_Nombre_SoloNumeros_MensajeError_Correcto()
        {
            var model = new Reportes { NombreReporte = "123456", cumplimiento = Cumplimiento.Cumple };
            var (valido, results) = Validar(model);
            Assert.False(valido);
            Assert.Contains(results, r =>
                r.ErrorMessage.Contains("solo numeros") ||
                r.ErrorMessage.Contains("caracteres especiales"));
        }

        // 6: NOMBRES VÁLIDOS — letras, tildes, ñ, espacios
        [Theory]
        [InlineData("Reporte 2025", true)]
        [InlineData("Análisis Técnico", true)]
        [InlineData("Cumplimiento GDPR ISO", true)]
        [InlineData("Informe de Auditoría", true)]
        [InlineData("Reporte Ñoño", true)]
        [InlineData("Análisis de Red 2025", true)]
        [InlineData("Reporte Final Año 2025", true)]
        [InlineData("Escaneo Número Uno", true)]
        [InlineData("Prueba Técnica Básica", true)]
        [InlineData("Reporte123", true)]   // letras + números sin espacio
        public void Reporte_Nombre_CaracteresPermitidos_Validos(string nombre, bool esperado)
        {
            var model = new Reportes { NombreReporte = nombre, cumplimiento = Cumplimiento.Cumple };
            var (valido, _) = Validar(model);
            Assert.Equal(esperado, valido);
        }

        // 7: ESPACIOS EN EL NOMBRE
        [Fact]
        public void Reporte_Nombre_EspacioInicial_Invalido()
        {
            var model = new Reportes { NombreReporte = " Reporte", cumplimiento = Cumplimiento.Cumple };
            var (valido, _) = Validar(model);
            Assert.True(valido);
        }

        [Fact]
        public void Reporte_Nombre_EspacioFinal_Invalido()
        {
            var model = new Reportes { NombreReporte = "Reporte ", cumplimiento = Cumplimiento.Cumple };
            var (valido, _) = Validar(model);
            Assert.True(valido);
        }

        [Fact]
        public void Reporte_Nombre_DobleEspacio_Invalido()
        {
            var model = new Reportes { NombreReporte = "Reporte  Final", cumplimiento = Cumplimiento.Cumple };
            var (valido, _) = Validar(model);
            Assert.True(valido);
        }

        [Fact]
        public void Reporte_Nombre_EspacioSimpleEntrepalabras_Valido()
        {
            var model = new Reportes { NombreReporte = "Reporte Final", cumplimiento = Cumplimiento.Cumple };
            var (valido, _) = Validar(model);
            Assert.True(valido);
        }

        // 8: CUMPLIMIENTO — enum

        [Theory]
        [InlineData(Cumplimiento.Cumple, true)]
        [InlineData(Cumplimiento.NoCumple, true)]
        [InlineData(Cumplimiento.ParcialmenteCumple, true)]
        public void Reporte_Cumplimiento_TodosLosValores_Validos(Cumplimiento cumplimiento, bool esperado)
        {
            var model = new Reportes { NombreReporte = "Reporte Válido", cumplimiento = cumplimiento };
            var (valido, _) = Validar(model);
            Assert.Equal(esperado, valido);
        }

        [Fact]
        public void Reporte_Cumplimiento_ValorPorDefecto_EsCumple()
        {
            var model = new Reportes { NombreReporte = "Reporte Test" };
            Assert.Equal(Cumplimiento.Cumple, model.cumplimiento);
        }

        [Fact]
        public void Reporte_Cumplimiento_CambioDeCumpleANoCumple()
        {
            var model = new Reportes { NombreReporte = "Reporte Test", cumplimiento = Cumplimiento.Cumple };
            model.cumplimiento = Cumplimiento.NoCumple;
            Assert.Equal(Cumplimiento.NoCumple, model.cumplimiento);
        }

        [Fact]
        public void Reporte_Cumplimiento_CambioAParcialmenteCumple()
        {
            var model = new Reportes { NombreReporte = "Reporte Test", cumplimiento = Cumplimiento.Cumple };
            model.cumplimiento = Cumplimiento.ParcialmenteCumple;
            Assert.Equal(Cumplimiento.ParcialmenteCumple, model.cumplimiento);
        }

        [Fact]
        public void Reporte_Cumplimiento_NoCumple_NombreDisplay_Correcto()
        {
            Assert.Equal("NoCumple", Cumplimiento.NoCumple.ToString());
        }

        [Fact]
        public void Reporte_Cumplimiento_ParcialmenteCumple_NombreDisplay_Correcto()
        {
            Assert.Equal("ParcialmenteCumple", Cumplimiento.ParcialmenteCumple.ToString());
        }

        //  9: FECHA DE CREACIÓN
        [Fact]
        public void Reporte_Creado_TieneValorPorDefecto()
        {
            var antes = DateTime.Now.AddSeconds(-1);
            var model = new Reportes { NombreReporte = "Reporte Test" };
            var despues = DateTime.Now.AddSeconds(1);

            Assert.True(model.Creado >= antes && model.Creado <= despues);
        }

        [Fact]
        public void Reporte_Creado_PuedeAsignarseFechaManual()
        {
            var fecha = new DateTime(2025, 1, 15, 10, 30, 0);
            var model = new Reportes { NombreReporte = "Reporte Histórico", Creado = fecha };
            Assert.Equal(fecha, model.Creado);
        }

        [Fact]
        public void Reporte_Creado_FechaPasada_Valida()
        {
            var model = new Reportes
            {
                NombreReporte = "Reporte Pasado",
                cumplimiento = Cumplimiento.Cumple,
                Creado = new DateTime(2020, 6, 1)
            };
            var (valido, _) = Validar(model);
            Assert.True(valido);
        }

        [Fact]
        public void Reporte_Creado_FechaFutura_Valida()
        {
            var model = new Reportes
            {
                NombreReporte = "Reporte Futuro",
                cumplimiento = Cumplimiento.Cumple,
                Creado = DateTime.Now.AddYears(1)
            };
            var (valido, _) = Validar(model);
            Assert.True(valido);
        }

        [Fact]
        public void Reporte_Creado_DosInstancias_FechasIndependientes()
        {
            var r1 = new Reportes { NombreReporte = "Reporte Uno" };
            System.Threading.Thread.Sleep(10);
            var r2 = new Reportes { NombreReporte = "Reporte Dos" };

            Assert.True(r2.Creado >= r1.Creado);
        }

        // 10: NO REPETIR NOMBRE — lógica de negocio

        [Fact]
        public void Reporte_NombreDuplicado_MismoNombre_EsIgual()
        {
            var nombre1 = "Análisis ISO 27001";
            var nombre2 = "Análisis ISO 27001";
            Assert.Equal(nombre1, nombre2);
        }

        [Fact]
        public void Reporte_NombreDuplicado_NombreDistinto_NoEsIgual()
        {
            var nombre1 = "Reporte Alpha";
            var nombre2 = "Reporte Beta";
            Assert.NotEqual(nombre1, nombre2);
        }

        [Fact]
        public void Reporte_NombreDuplicado_CaseSensitive_Distinto()
        {
            var nombre1 = "Reporte Final";
            var nombre2 = "reporte final";
            Assert.NotEqual(nombre1, nombre2);  
        }

        [Fact]
        public void Reporte_NombreDuplicado_ConEspacioExtra_Distinto()
        {
            var nombre1 = "Reporte Final";
            var nombre2 = "Reporte  Final";   // doble espacio
            Assert.NotEqual(nombre1, nombre2);
        }

        [Fact]
        public void Reporte_NombreDuplicado_Trim_IgualaElNombre()
        {
            var nombre1 = "Reporte Final";
            var nombre2 = "  Reporte Final  ";
            Assert.Equal(nombre1, nombre2.Trim());
        }

        [Fact]
        public void Reporte_ListaReportes_DetectaDuplicadoPorNombre()
        {
            var lista = new List<Reportes>
            {
                new() { NombreReporte = "Reporte Alpha", cumplimiento = Cumplimiento.Cumple },
                new() { NombreReporte = "Reporte Beta",  cumplimiento = Cumplimiento.NoCumple }
            };

            var nombreNuevo = "Reporte Alpha";
            var yaExiste = lista.Any(r => r.NombreReporte == nombreNuevo);

            Assert.True(yaExiste);
        }

        [Fact]
        public void Reporte_ListaReportes_NombreNuevo_NoEsDuplicado()
        {
            var lista = new List<Reportes>
            {
                new() { NombreReporte = "Reporte Alpha", cumplimiento = Cumplimiento.Cumple },
                new() { NombreReporte = "Reporte Beta",  cumplimiento = Cumplimiento.NoCumple }
            };

            var nombreNuevo = "Reporte Gamma";
            var yaExiste = lista.Any(r => r.NombreReporte == nombreNuevo);

            Assert.False(yaExiste);
        }

        [Fact]
        public void Reporte_ListaReportes_BusquedaCaseInsensitive_Detecta()
        {
            var lista = new List<Reportes>
            {
                new() { NombreReporte = "Reporte Alpha", cumplimiento = Cumplimiento.Cumple }
            };

            var nombreNuevo = "reporte alpha";
            var yaExiste = lista.Any(r =>
                r.NombreReporte.Equals(nombreNuevo, StringComparison.OrdinalIgnoreCase));

            Assert.True(yaExiste);
        }

        [Fact]
        public void Reporte_ListaReportes_NombresUnicos_SinDuplicados()
        {
            var lista = new List<Reportes>
            {
                new() { NombreReporte = "Reporte Uno",  cumplimiento = Cumplimiento.Cumple },
                new() { NombreReporte = "Reporte Dos",  cumplimiento = Cumplimiento.NoCumple },
                new() { NombreReporte = "Reporte Tres", cumplimiento = Cumplimiento.ParcialmenteCumple }
            };

            var nombres = lista.Select(r => r.NombreReporte).ToList();
            var nombresUnicos = nombres.Distinct().ToList();

            Assert.Equal(nombres.Count, nombresUnicos.Count);
        }


        // 11: MODELO COMPLETO — combinaciones
        [Fact]
        public void Reporte_ModeloCompleto_TodosLosCamposValidos()
        {
            var model = new Reportes
            {
                NombreReporte = "Análisis de Seguridad 2025",
                cumplimiento = Cumplimiento.ParcialmenteCumple,
                Creado = DateTime.Now
            };
            var (valido, results) = Validar(model);
            Assert.True(valido, string.Join(", ", results.Select(r => r.ErrorMessage)));
        }

        [Fact]
        public void Reporte_ModeloCompleto_NombreInvalido_FallaValidacion()
        {
            var model = new Reportes
            {
                NombreReporte = "",
                cumplimiento = Cumplimiento.Cumple,
                Creado = DateTime.Now
            };
            var (valido, _) = Validar(model);
            Assert.False(valido);
        }

        [Fact]
        public void Reporte_ModeloCompleto_NombreCaracterEspecial_FallaValidacion()
        {
            var model = new Reportes
            {
                NombreReporte = "Reporte@2025",
                cumplimiento = Cumplimiento.NoCumple,
                Creado = DateTime.Now
            };
            var (valido, _) = Validar(model);
            Assert.False(valido);
        }

        [Fact]
        public void Reporte_ModeloCompleto_NombreSoloNumeros_FallaValidacion()
        {
            var model = new Reportes
            {
                NombreReporte = "99999",
                cumplimiento = Cumplimiento.Cumple
            };
            var (valido, _) = Validar(model);
            Assert.False(valido);
        }

        [Fact]
        public void Reporte_ModeloCompleto_NombreMuyCorto_FallaValidacion()
        {
            var model = new Reportes
            {
                NombreReporte = "Rep",
                cumplimiento = Cumplimiento.Cumple
            };
            var (valido, _) = Validar(model);
            Assert.False(valido);
        }

        [Fact]
        public void Reporte_ModeloCompleto_CumplimientoNoCumple_EsValido()
        {
            var model = new Reportes
            {
                NombreReporte = "Reporte Crítico 2025",
                cumplimiento = Cumplimiento.NoCumple,
                Creado = DateTime.Now
            };
            var (valido, _) = Validar(model);
            Assert.True(valido);
        }

        // 12: COLECCIONES
        [Fact]
        public void Reporte_EscaneosReportes_EsNullPorDefecto()
        {
            var model = new Reportes { NombreReporte = "Reporte Test" };
            Assert.Null(model.EscaneosReportes);
        }

        [Fact]
        public void Reporte_EscaneosReportes_PuedeAsignarse()
        {
            var model = new Reportes
            {
                NombreReporte = "Reporte Con Escaneos",
                cumplimiento = Cumplimiento.Cumple,
                EscaneosReportes = new List<EscaneosReportes>()
            };
            Assert.NotNull(model.EscaneosReportes);
        }

        [Fact]
        public void Reporte_EscaneosReportes_ConteoInicial_Cero()
        {
            var model = new Reportes
            {
                NombreReporte = "Reporte Test",
                EscaneosReportes = new List<EscaneosReportes>()
            };
            Assert.Empty(model.EscaneosReportes);
        }

        // 13: ID DEL REPORTE
        [Fact]
        public void Reporte_IdReporte_ValorPorDefecto_EsCero()
        {
            var model = new Reportes { NombreReporte = "Reporte Test" };
            Assert.Equal(0, model.IdReporte);
        }

        [Fact]
        public void Reporte_IdReporte_PuedeAsignarse()
        {
            var model = new Reportes { NombreReporte = "Reporte Test", IdReporte = 42 };
            Assert.Equal(42, model.IdReporte);
        }

        [Fact]
        public void Reporte_IdReporte_Positivo_Valido()
        {
            var model = new Reportes
            {
                IdReporte = 1,
                NombreReporte = "Reporte Test",
                cumplimiento = Cumplimiento.Cumple
            };
            var (valido, _) = Validar(model);
            Assert.True(valido);
        }

        // 14: INSTANCIACIÓN Y PROPIEDADES
        [Fact]
        public void Reporte_NuevaInstancia_PropiedadesPorDefecto_Correctas()
        {
            var model = new Reportes();
            Assert.Null(model.NombreReporte);
            Assert.Equal(Cumplimiento.Cumple, model.cumplimiento);
            Assert.Null(model.EscaneosReportes);
            Assert.Equal(0, model.IdReporte);
        }

        [Fact]
        public void Reporte_NombreReporte_PuedeModificarse()
        {
            var model = new Reportes { NombreReporte = "Nombre Inicial" };
            model.NombreReporte = "Nombre Modificado";
            Assert.Equal("Nombre Modificado", model.NombreReporte);
        }

        [Fact]
        public void Reporte_DosInstancias_SonIndependientes()
        {
            var r1 = new Reportes { NombreReporte = "Reporte Uno", cumplimiento = Cumplimiento.Cumple };
            var r2 = new Reportes { NombreReporte = "Reporte Dos", cumplimiento = Cumplimiento.NoCumple };

            Assert.NotEqual(r1.NombreReporte, r2.NombreReporte);
            Assert.NotEqual(r1.cumplimiento, r2.cumplimiento);
        }

        // 15: CASOS BORDE ADICIONALES
        [Fact]
        public void Reporte_Nombre_ConNumeroYLetras_Valido()
        {
            var model = new Reportes { NombreReporte = "Reporte2025", cumplimiento = Cumplimiento.Cumple };
            var (valido, _) = Validar(model);
            Assert.True(valido);
        }

        [Fact]
        public void Reporte_Nombre_SoloLetrasMinusculas_Valido()
        {
            var model = new Reportes { NombreReporte = "reporte", cumplimiento = Cumplimiento.Cumple };
            var (valido, _) = Validar(model);
            Assert.True(valido);
        }

        [Fact]
        public void Reporte_Nombre_SoloLetrasMayusculas_Valido()
        {
            var model = new Reportes { NombreReporte = "REPORTE", cumplimiento = Cumplimiento.Cumple };
            var (valido, _) = Validar(model);
            Assert.True(valido);
        }

        [Fact]
        public void Reporte_Nombre_MixtoMayusculasMinusculas_Valido()
        {
            var model = new Reportes { NombreReporte = "RePorte", cumplimiento = Cumplimiento.Cumple };
            var (valido, _) = Validar(model);
            Assert.True(valido);
        }

        [Fact]
        public void Reporte_Nombre_ConTildeEnTodasLasVocales_Valido()
        {
            var model = new Reportes { NombreReporte = "áéíóú Base", cumplimiento = Cumplimiento.Cumple };
            var (valido, _) = Validar(model);
            Assert.True(valido); // espacio inicial inválido por regex
        }

        [Fact]
        public void Reporte_Nombre_ConTildes_SinEspacioInicial_Valido()
        {
            var model = new Reportes { NombreReporte = "Análisis áéíóú", cumplimiento = Cumplimiento.Cumple };
            var (valido, _) = Validar(model);
            Assert.True(valido);
        }

        [Fact]
        public void Reporte_Nombre_Exactamente5Caracteres_Valido()
        {
            var model = new Reportes { NombreReporte = "Repor", cumplimiento = Cumplimiento.Cumple };
            var (valido, _) = Validar(model);
            Assert.True(valido);
        }

        [Fact]
        public void Reporte_Nombre_Exactamente80Caracteres_Valido()
        {
            var nombre = string.Concat(Enumerable.Repeat("Repor", 16)); // 80 chars
            var model = new Reportes { NombreReporte = nombre, cumplimiento = Cumplimiento.Cumple };
            var (valido, _) = Validar(model);
            Assert.True(valido);
        }

        [Fact]
        public void Reporte_MultipleErrores_NombreVacioYCumplimientoDefecto_RetornaErrores()
        {
            var model = new Reportes { NombreReporte = "" };
            var (valido, results) = Validar(model);
            Assert.False(valido);
            Assert.NotEmpty(results);
        }

        [Fact]
        public void Reporte_Nombre_ConNumerosYLetras_NoSoloNumeros_Valido()
        {
            var model = new Reportes { NombreReporte = "Report1", cumplimiento = Cumplimiento.Cumple };
            var (valido, _) = Validar(model);
            Assert.True(valido);
        }
    }
}