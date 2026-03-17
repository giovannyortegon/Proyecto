using AuditSentinel.Models;
using System.ComponentModel.DataAnnotations;

namespace AuditSentinel.Test
{
    public class RegistroTests
    {
        private static (bool valido, List<ValidationResult> results) Validar(object model)
        {
            var ctx = new ValidationContext(model);
            var results = new List<ValidationResult>();
            var valido = Validator.TryValidateObject(model, ctx, results, validateAllProperties: true);
            return (valido, results);
        }

        [Theory]
        [InlineData("", false)]
        [InlineData("Juan", true)]
        [InlineData("María José", true)]
        [InlineData("Ñuñez", true)]
        [InlineData(" Juan", false)]         // espacio inicial
        [InlineData("Juan  Pablo", false)]   // doble espacio
        [InlineData("Juan@", false)]         // caracter especial
        [InlineData("Ana_12", false)]        // guion bajo y números
        [InlineData("Ana", false)]           // si MinLength=4

        public void Registro_Nombre_Obligatorio(string nombre, bool ValidadorEsperado)
        {
            var model = new Registro
            {
                Nombre = nombre,
                Apellido = "Perez",
                Email = "correo@correo.com",
                Password = "Prueba2024*",
                ConfirmacionPassword = "Prueba2024*",
                Rol = new List<string> { "Administrador" }
            };

            var (valido, _) = Validar(model);
            Assert.Equal(ValidadorEsperado, valido);
        }

        [Theory]
        [InlineData("", false)]
        [InlineData("Perez Rodriguez", true)]
        [InlineData("Perez", true)]
        [InlineData(" Perez", false)]         // espacio inicial
        [InlineData("Perez  Rodriguez", false)]   // doble espacio
        [InlineData("Perez@", false)]         // caracter especial
        [InlineData("Perez_12", false)]        // guion bajo y números
        [InlineData("Per", false)]           // si MinLength=4

        public void Registro_Apellido_Obligatorio(string apellido, bool ValidadorEsperado)
        {
            var model = new Registro
            {
                Nombre = "Juan",
                Apellido = apellido,
                Email = "correo@correo.com",
                Password = "Prueba2024*",
                ConfirmacionPassword = "Prueba2024*",
                Rol = new List<string> { "Administrador" }
            };

            var (valido, _) = Validar(model);
            Assert.Equal(ValidadorEsperado, valido);
        }

        [Theory]
        [InlineData("Juan", "Perez", true)]
        [InlineData("", "Prez", false)]
        [InlineData("Juan", "", false)]
        public void Registro_Nombre_Apellido_Obligatorio(string nombre, string apellido, bool ValidadorEsperado)
        {
            var model = new Registro
            {
                Nombre = nombre,
                Apellido = apellido,
                Email = "correo@correo.com",
                Password = "Prueba2024*",
                ConfirmacionPassword = "Prueba2024*",
                Rol = new List<string> { "Administrador" }
            };

            var (valido, _) = Validar(model);
            Assert.Equal(ValidadorEsperado, valido);
        }

        [Theory]
        [InlineData("", false)]
        [InlineData("sdfsdfas@", false)]
        [InlineData("correo@empresa", false)]
        [InlineData("correo@@empresa.com", false)]
        [InlineData("correo@correo.com", true)]
        public void Registro_Email(string email, bool ValidadorEsperado)
        {
            var model = new Registro
            {
                Nombre = "Juan",
                Apellido = "Rodriguez",
                Email = email,
                Password = "Prueba2024*",
                ConfirmacionPassword = "Prueba2024*",
                Rol = new List<string> { "Administrador" }
            };

            var (valido, _) = Validar(model);
            Assert.Equal(ValidadorEsperado, valido);
        }

        [Theory]
        [InlineData("123456Qa@", "123456Qa@", true)]
        [InlineData("123456A@", "000000A@", false)]
        [InlineData("passworD2!@", "passworD2!@", true)]
        [InlineData("password1@", "passw0rd1@", false)]
        public void Registro_Contrasena_NoCoincide(string pass, string confpass, bool ValidadorEsperado)
        {
            var model = new Registro
            {
                Nombre = "Juan",
                Apellido = "Rodriguez",
                Email = "correo@correo.com",
                Password = pass,
                ConfirmacionPassword = confpass,
                Rol = new List<string> { "Administrador" }
            };

            var (valido, _) = Validar(model);
            Assert.Equal(ValidadorEsperado, valido);
        }


        [Theory]
        [InlineData("Abcdef1!", true)]  // OK (mayús, minús, numero, especial, ≥8)
        [InlineData("Prueba2024*", true)]  
        [InlineData("HolaMundo1@", true)]  
        [InlineData("XyZ123$%", true)]  
        [InlineData("abcdefg!", false)]// sin mayúsculas
        [InlineData("ABCDEFG!", false)]// sin minúsculas
        [InlineData("Abcdefgh", false)]// sin número ni especial
        [InlineData("Abc12345", false)]// sin especial
        [InlineData("Abc!12", false)]// menos de 8
        public void Registro_Politicas_Contrasena(string pass, bool ValidadorEsperado)
        {
            var model = new Registro
            {
                Nombre = "Juan",
                Apellido = "Rodriguez",
                Email = "correo@correo.com",
                Password = pass,
                ConfirmacionPassword = pass,
                Rol = new List<string> { "Administrador" }
            };

            var (valido, _) = Validar(model);
            Assert.Equal(ValidadorEsperado, valido);
        }
        [Fact]
        public void Registro_Rol()
        {
            var model = new Registro
            {
                Nombre = "Juan",
                Apellido = "Perez",
                Email = "correo@correo.com",
                Password = "Prueba2024*",
                ConfirmacionPassword = "Prueba2024*",
                Rol = null
            };

            var (valido, results) = Validar(model);

            Assert.False(valido);
            Assert.Contains(results, r => r.ErrorMessage.Contains("El rol es obligatoria"));
        }

    }

}
