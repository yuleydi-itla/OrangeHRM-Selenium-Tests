using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using OrangeHRM.Tests.Base;

namespace OrangeHRM.Tests.Tests
{
    [TestFixture]
    public class CreateEmployeeTests : BaseTest
    {
        private const string LoginUrl = "https://opensource-demo.orangehrmlive.com/web/index.php/auth/login";
        private const string AddEmpUrl = "https://opensource-demo.orangehrmlive.com/web/index.php/pim/addEmployee";

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

        //Camino feliz
        [Test]
        public void CrearEmpleado_DatosValidos_SeGuardaCorrectamente()
        {
            test = extent.CreateTest("HU-02 - Crear empleado (válido)")
              .AssignCategory("HU-02 Crear Empleado")
              .AssignAuthor("Yuleydi De Los Santos");

            test.Info("Inicio de prueba: Crear empleado");

            DoLogin();
            test.Info("Login realizado correctamente");

            driver.Navigate().GoToUrl("https://opensource-demo.orangehrmlive.com/web/index.php/pim/addEmployee");
            test.Info("Accediendo al formulario de creación de empleado");

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));

            // Esperar campo visible
            wait.Until(d => d.FindElement(By.Name("firstName")).Displayed);

            driver.FindElement(By.Name("firstName")).Clear();
            driver.FindElement(By.Name("firstName")).SendKeys("Maria");
            driver.FindElement(By.Name("middleName")).SendKeys("Test");
            driver.FindElement(By.Name("lastName")).SendKeys("Automation");

            // Click en guardar
            ClickWhenReady(By.CssSelector("button[type='submit']"));

            test.Info("Formulario enviado");

            
            wait.Until(d => d.FindElement(By.XPath("//h6[text()='Personal Details']")).Displayed);

            test.Info("Redirección a detalles personales confirmada");

            TakeScreenshot("CrearEmpleado_CaminoFeliz");

            Assert.That(
                driver.FindElement(By.XPath("//h6[text()='Personal Details']")).Displayed,
                Is.True,
                "No se mostró la pantalla de detalles personales");

            test.Pass("Empleado creado correctamente");
        }

        // Prueba negativa
        [Test]
        public void CrearEmpleado_SinNombre_MuestraValidacion()
        {
            test = extent.CreateTest("HU- 02 - Crear empleado(sin nombre)")
               .AssignCategory("HU-02 Crear Empleado")
               .AssignAuthor("Yuleydi De Los Santos");
            test.Info("Inicio de prueba negativa: creación sin nombre");
            DoLogin();
            test.Info("Login realizado correctamente");
            driver.Navigate().GoToUrl(AddEmpUrl);
            test.Info("Accediendo al formulario de creación");

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
            wait.Until(d => d.FindElement(By.Name("firstName")).Displayed);
            Thread.Sleep(1500);

            ClickWhenReady(By.CssSelector("button[type='submit']"));
            test.Info("Intentando guardar sin datos obligatorios");

            var errors = wait.Until(d =>
                d.FindElements(By.CssSelector(".oxd-input-field-error-message")));

            TakeScreenshot("HU02_CrearEmpleado_Error");
            Assert.That(errors.Count, Is.GreaterThan(0));
            test.Pass("Validación mostrada correctamente");
        }

        //Prueba de límites
        [Test]
        public void CrearEmpleado_NombreMuyLargo_MuestraValidacion()
        {
            test = extent.CreateTest("HU-02 - Crear empleado(nombre muy largo)")
                .AssignCategory("HU-02 Crear Empleado")
                .AssignAuthor("Yuleydi De Los Santos");
            test.Info("Inicio de prueba de límite de caracteres");
            DoLogin();
            test.Info("Login realizado correctamente");
            driver.Navigate().GoToUrl(AddEmpUrl);
            test.Info("Accediendo al formulario de creación");


            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
            wait.Until(d => d.FindElement(By.Name("firstName")).Displayed);
            Thread.Sleep(1500);

            driver.FindElement(By.Name("firstName")).SendKeys(new string('A', 200));
            test.Info("Se ingresa un nombre con 200 caracteres");
            driver.FindElement(By.Name("lastName")).SendKeys("Test");

            ClickWhenReady(By.CssSelector("button[type='submit']"));
            test.Info("Intentando guardar con datos fuera del límite");

            Thread.Sleep(2000);
            TakeScreenshot("HU02_CrearEmpleado_Limite");

            var errors = driver.FindElements(
                By.CssSelector(".oxd-input-field-error-message"));

            Assert.That(errors.Count, Is.GreaterThan(0));
            test.Pass("Límite de caracteres validado correctamente");
        }
    }
}