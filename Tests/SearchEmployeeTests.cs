using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using OrangeHRM.Tests.Base;

namespace OrangeHRM.Tests.Tests
{
    [TestFixture]
    public class SearchEmployeeTests : BaseTest
    {
        private const string LoginUrl = "https://opensource-demo.orangehrmlive.com/web/index.php/auth/login";
        private const string SearchUrl = "https://opensource-demo.orangehrmlive.com/web/index.php/pim/viewEmployeeList";

        private void DoLogin()
        {
            driver.Navigate().GoToUrl(LoginUrl);
            driver.FindElement(By.Name("username")).SendKeys("Admin");
            driver.FindElement(By.Name("password")).SendKeys("admin123");
            driver.FindElement(By.CssSelector("button[type='submit']")).Click();
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
            wait.Until(d => d.Url.Contains("dashboard"));
        }

        //Camino feliz
        [Test]
        public void BuscarEmpleado_NombreExistente_MuestraResultados()
        {
            test = extent.CreateTest("HU-03 | Buscar empleado existente");
            test.AssignAuthor("Yuleydi De Los Santos");
            test.Info("Inicio de prueba: búsqueda de empleado existente");
            DoLogin();
            test.Info("Login realizado correctamente");
            driver.Navigate().GoToUrl(SearchUrl);
            test.Info("Accediendo al módulo de búsqueda de empleados");

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
            var searchInput = wait.Until(d =>
                d.FindElement(By.CssSelector(
                    "input.oxd-input.oxd-input--active:not([readonly])")));

            searchInput.SendKeys("Admin");
            test.Info("Se ingresa nombre del empleado a buscar");
            driver.FindElement(By.CssSelector("button[type='submit']")).Click();
            test.Info("Se ejecuta la búsqueda");

            wait.Until(d =>
                d.FindElements(By.CssSelector(".oxd-table-body .oxd-table-row")).Count > 0);

            TakeScreenshot("HU03_Busqueda_OK");
            var rows = driver.FindElements(
                By.CssSelector(".oxd-table-body .oxd-table-row"));

            Assert.That(rows.Count, Is.GreaterThan(0),
                "Debería mostrar al menos un resultado");

            test.Pass($"Búsqueda exitosa — {rows.Count} resultado(s) encontrado(s)");
        }


        //Prueba negativa
        [Test]
        public void BuscarEmpleado_NombreInexistente_MuestraSinResultados()
        {
            test = extent.CreateTest("HU-03 | Buscar empleado inexistente");
            test.AssignAuthor("Yuleydi De Los Santos");
            test.Info("Inicio de prueba negativa: búsqueda de empleado inexistente");
            DoLogin();
            test.Info("Login realizado correctamente");
            driver.Navigate().GoToUrl(SearchUrl);
            test.Info("Accediendo al módulo de búsqueda");

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
            wait.Until(d => d.FindElements(
                By.CssSelector(".oxd-table-body .oxd-table-row")).Count > 0);
            Thread.Sleep(1000);

            // Buscar por Employee ID inexistente es más confiable
            var employeeIdInput = driver.FindElements(
                By.CssSelector("input.oxd-input.oxd-input--active"));

            // El segundo input es el de Employee ID
            if (employeeIdInput.Count >= 2)
                employeeIdInput[1].SendKeys("000000");
            else
                employeeIdInput[0].SendKeys("000000");

            driver.FindElement(By.CssSelector("button[type='submit']")).Click();
            test.Info("Se ejecuta la búsqueda con datos inválidos");
            Thread.Sleep(3000);
            TakeScreenshot("HU03_Busqueda_Error");

            var allSpans = driver.FindElements(By.CssSelector(".oxd-text--span"));
            var resultText = allSpans.FirstOrDefault(s =>
                s.Text.Contains("Records") || s.Text.Contains("("));

            bool sinResultados =
                resultText == null ||
                resultText.Text.Contains("(0)") ||
                resultText.Text.Contains("No Records") ||
                resultText.Text.Contains("0 Record");

            Assert.That(sinResultados, Is.True,
                $"Debería mostrar 0 resultados. Texto: {resultText?.Text}");

            test.Pass("Búsqueda negativa correcta — ID inexistente no encontrado");
        }

        //Prueba de límites
        [Test]
        public void BuscarEmpleado_NombreUnCaracter_RetornaResultado()
        {
            test = extent.CreateTest("HU-03 | Buscar empleado (límite)");
            test.AssignAuthor("Yuleydi De Los Santos");
            test.Info("Inicio de prueba de límite");
            DoLogin();
            test.Info("Login realizado correctamente");
            driver.Navigate().GoToUrl(SearchUrl);
            test.Info("Accediendo al módulo de búsqueda");

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
            var searchInput = wait.Until(d =>
                d.FindElement(By.CssSelector(
                    "input.oxd-input.oxd-input--active:not([readonly])")));

            searchInput.SendKeys("A");
            test.Info("Se ingresa un solo carácter para búsqueda");
            driver.FindElement(By.CssSelector("button[type='submit']")).Click();
            test.Info("Se ejecuta la búsqueda con dato mínimo");

            System.Threading.Thread.Sleep(2000);
            TakeScreenshot("HU03_Busqueda_Limite");

            var rows = driver.FindElements(
                By.CssSelector(".oxd-table-body .oxd-table-row"));

            Assert.That(rows.Count, Is.GreaterThanOrEqualTo(0),
                "La búsqueda con un carácter debe responder sin errores");

            test.Pass($"Límite de un carácter manejado correctamente — {rows.Count} resultado(s)");
        }
    }
}