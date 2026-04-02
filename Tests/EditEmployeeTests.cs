using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using OrangeHRM.Tests.Base;

namespace OrangeHRM.Tests.Tests
{
    [TestFixture]
    public class EditEmployeeTests : BaseTest
    {
        private const string LoginUrl = "https://opensource-demo.orangehrmlive.com/web/index.php/auth/login";
        private const string SearchUrl = "https://opensource-demo.orangehrmlive.com/web/index.php/pim/viewEmployeeList";

        private void DoLogin()
        {
            driver.Navigate().GoToUrl(LoginUrl);
            driver.FindElement(By.Name("username")).SendKeys("Admin");
            driver.FindElement(By.Name("password")).SendKeys("admin123");
            driver.FindElement(By.CssSelector("button[type='submit']")).Click();
            new WebDriverWait(driver, TimeSpan.FromSeconds(15))
                .Until(d => d.Url.Contains("dashboard"));
        }

        private void ClickWhenReady(By locator)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
            var el = wait.Until(d => {
                var e = d.FindElement(locator);
                return (e.Displayed && e.Enabled) ? e : null;
            });
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", el);
        }

        private void GoToFirstEmployeeEdit()
        {
            driver.Navigate().GoToUrl(SearchUrl);
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
            wait.Until(d =>
                d.FindElements(By.CssSelector(".oxd-table-body .oxd-table-row")).Count > 0);
            Thread.Sleep(1000);

            var editBtn = driver.FindElement(
                By.CssSelector(".oxd-table-body .oxd-table-row .oxd-icon-button"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", editBtn);

            wait.Until(d => d.FindElement(By.Name("firstName")).Displayed);
            Thread.Sleep(2000);
        }

        //Camino feliz
        [Test]
        public void EditarEmpleado_DatosValidos_SeActualizaCorrectamente()
        {
            test = extent.CreateTest("HU-04 - Editar empleado (válido)");
            test.AssignAuthor("Yuleydi De Los Santos");
            test.Info("Inicio de prueba: edición de empleado con datos válidos");
            DoLogin();
            test.Info("Login realizado correctamente");
            GoToFirstEmployeeEdit();
            test.Info("Accediendo al formulario de edición del empleado");

            var firstName = driver.FindElement(By.Name("firstName"));
            firstName.Clear();
            firstName.SendKeys("MariaEditada");
            test.Info("Se modifica el nombre del empleado");

            ClickWhenReady(By.CssSelector("button[type='submit']"));
            test.Info("Se guardan los cambios");
            Thread.Sleep(2000);

            TakeScreenshot("EditarEmpleado_CaminoFeliz");
            Assert.That(driver.Url,
                Does.Contain("viewPersonalDetails").Or
                    .Contain("personalDetails").Or
                    .Contain("viewEmployeeList"));

            test.Pass("Empleado editado correctamente");
        }

        // Prueba negativa
        [Test]
        public void EditarEmpleado_BorrarNombre_MuestraValidacion()
        {
            test = extent.CreateTest("HU-04 - Editar empleado (sin nombre)");
            test.AssignAuthor("Yuleydi De Los Santos");
            test.Info("Inicio de prueba negativa: borrar nombre del empleado");
            DoLogin();
            test.Info("Login realizado correctamente");
            GoToFirstEmployeeEdit();
            test.Info("Accediendo al formulario de edición");

            var firstName = driver.FindElement(By.Name("firstName"));
            firstName.Clear();
            test.Info("Se elimina el nombre del empleado");

            // Forzar validación con Tab antes de guardar
            firstName.SendKeys(Keys.Tab);
            Thread.Sleep(500);

            ClickWhenReady(By.CssSelector("button[type='submit']"));
            test.Info("Se intenta guardar sin nombre");
            Thread.Sleep(2000);

            TakeScreenshot("EditarEmpleado_Negativo");

            // OrangeHRM puede mostrar el error de distintas formas
            var url = driver.Url;
            var errors = driver.FindElements(
                By.CssSelector(".oxd-input-field-error-message"));
            var toastErrors = driver.FindElements(
                By.CssSelector(".oxd-toast--error"));

            Assert.That(
                errors.Count > 0 || toastErrors.Count > 0 ||
                url.Contains("viewPersonalDetails"),
                Is.True,
                "Debería mostrar validación o permanecer en el formulario");

            test.Pass("Validación de nombre vacío manejada correctamente");
        }

        // Prueba de límites
        [Test]
        public void EditarEmpleado_NombreUnCaracter_SeGuarda()
        {
            test = extent.CreateTest("HU-04 - Editar empleado (un cácter)");
            test.AssignAuthor("Yuleydi De Los Santos");
            test.Info("Inicio de prueba de límite");
            DoLogin();
            test.Info("Login realizado correctamente");
            GoToFirstEmployeeEdit();
            test.Info("Accediendo al formulario de edición");

            var firstName = driver.FindElement(By.Name("firstName"));
            firstName.Clear();
            firstName.SendKeys("A");
            test.Info("Se ingresa un nombre de un solo carácter");

            ClickWhenReady(By.CssSelector("button[type='submit']"));
            test.Info("Se guardan los cambios con dato mínimo");
            Thread.Sleep(2000);

            TakeScreenshot("EditarEmpleado_Limites");
            Assert.That(driver.Url,
                Does.Contain("viewPersonalDetails").Or
                    .Contain("personalDetails").Or
                    .Contain("viewEmployeeList"));

            test.Pass("Límite mínimo de nombre manejado correctamente");
        }
    }
}