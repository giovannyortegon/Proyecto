using AuditSentinel.Models;
using System.ComponentModel.DataAnnotations;

namespace AuditSentinel.Test
{
    public class LoginTest
    {
        private string emailDb = "gortegon@gmail.com";
        private string passwordDb = "Prueba2024*";
        private static (bool valido, List<ValidationResult> results) Validar(object model)
        {
            var ctx = new ValidationContext(model);
            var results = new List<ValidationResult>();
            var valido = Validator.TryValidateObject(model, ctx, results, validateAllProperties: true);
            return (valido, results);
        }

        [Fact]
        public void Login_Modelo_Invalido_SinEmail()
        {
            var model = new Login
            {
                Email = "",
                Password = "123456",
                RememberMe = false
            };

            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();

            var valido = Validator.TryValidateObject(model, context, results, true);

            Assert.False(valido);
            Assert.Contains(results, r => r.ErrorMessage.Contains("El correo electronico es obligatorio"));
        }


        [Theory]
        [InlineData("vsara@gmail.com", "123456", false)]         // email y pass no coinciden
        [InlineData("gortegon@gmail.com", "123456", false)]      // email ok, pass no
        [InlineData("vsara@gmail.com", "Prueba2024*", false)]    // pass ok, email no
        [InlineData("gortegon@gmail.com", "Prueba2024*", true)]  // ambos coinciden
        public void Login_Validacion_Credenciales_DebenCoincidirConAlmacenadas(
                string email, string password, bool esperadoValido)
        {
            // Arrange
            var model = new Login
            {
                Email = email,
                Password = password,
                RememberMe = false
            };

            var (modeloValido, _) = Validar(model);
            Assert.True(modeloValido); 
            bool credencialesOk = model.Email == emailDb && model.Password == passwordDb;
            Assert.Equal(esperadoValido, credencialesOk);
        }


    }
}
