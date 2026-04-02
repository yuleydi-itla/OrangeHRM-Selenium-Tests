using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using OrangeHRM.Tests.Base;

namespace OrangeHRM.Tests.Tests
{
    [TestFixture]
    public class LoginTests : BaseTest
    {
        private const string Url = "https://opensource-demo.orangehrmlive.com/web/index.php/auth/login";
        private const string ValidUser = "Admin";
        private const string ValidPass = "admin123";

        // Camino feliz
        [Test]
        public void Login_CredencialesValidas_RedirigeDashboard()
        {
            test = extent.CreateTest("HU-01 - Login (válido)")
              .AssignCategory("HU-01 Login")
              .AssignAuthor("Yuleydi De Los Santos");
            test.Info("Se abre la página de login");
            driver.Navigate().GoToUrl(Url);

            driver.FindElement(By.Name("username")).SendKeys(ValidUser);
            test.Info("Se ingresa usuario");
            driver.FindElement(By.Name("password")).SendKeys(ValidPass);
            test.Info("Se ingresa contraseña");
            driver.FindElement(By.CssSelector("button[type='submit']")).Click();
            test.Info("Se hace clic en login");

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
            wait.Until(d => d.Url.Contains("dashboard"));

            TakeScreenshot("Login_CaminoFeliz");
            Assert.That(driver.Url, Does.Contain("dashboard"),
                "Debería redirigir al dashboard tras login exitoso");

            test.Pass("Login exitoso — redirigió al dashboard correctamente");
        }

        // Prueba negativa
        [Test]
        public void Login_CredencialesInvalidas_MuestraError()
        {
            test = extent.CreateTest("HU-01 — Login (inválido)");
            driver.Navigate().GoToUrl(Url);

            driver.FindElement(By.Name("username")).SendKeys("usuarioFalso");
            driver.FindElement(By.Name("password")).SendKeys("claveErronea");
            driver.FindElement(By.CssSelector("button[type='submit']")).Click();

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            var error = wait.Until(d =>
                d.FindElement(By.CssSelector(".oxd-alert-content-text")));

            TakeScreenshot("Login_Negativo");
            Assert.That(error.Text, Does.Contain("Invalid credentials"),
                "Debería mostrar mensaje de error con credenciales inválidas");

            test.Pass("Prueba negativa exitosa — mostró mensaje de error correctamente");
        }

        // Prueba de límites
        [Test]
        public void Login_CamposVacios_MuestraValidacion()
        {
            test = extent.CreateTest("HU-01 — Login (campos vacíos)");
            driver.Navigate().GoToUrl(Url);

            driver.FindElement(By.CssSelector("button[type='submit']")).Click();

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            var errors = wait.Until(d =>
                d.FindElements(By.CssSelector(".oxd-input-field-error-message")));

            TakeScreenshot("Login_Limites");
            Assert.That(errors.Count, Is.GreaterThan(0),
                "Debería mostrar validaciones con campos vacíos");

            test.Pass("Prueba de límites exitosa — validaciones mostradas correctamente");
        }
    }
}
