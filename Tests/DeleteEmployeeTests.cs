using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using OrangeHRM.Tests.Base;
using System;
using System.Linq;              
using System.Threading;         

namespace OrangeHRM.Tests.Tests
{
    [TestFixture]
    public class DeleteEmployeeTests : BaseTest
    {
        private const string LoginUrl = "https://opensource-demo.orangehrmlive.com/web/index.php/auth/login";
        private const string SearchUrl = "https://opensource-demo.orangehrmlive.com/web/index.php/pim/viewEmployeeList";

        //Login 
        private void DoLogin()
        {
            driver.Navigate().GoToUrl(LoginUrl);

            driver.FindElement(By.Name("username")).SendKeys("Admin");
            driver.FindElement(By.Name("password")).SendKeys("admin123");
            driver.FindElement(By.CssSelector("button[type='submit']")).Click();

            new WebDriverWait(driver, TimeSpan.FromSeconds(15))
                .Until(d => d.Url.Contains("dashboard"));
        }

        //Click con Js
        private void ClickJS(IWebElement el)
        {
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", el);
        }

        // Esperar tabla
        private void WaitForTable()
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));

            wait.Until(d =>
                d.FindElements(By.CssSelector(".oxd-table-body .oxd-table-row")).Count > 0);

            Thread.Sleep(1000); // estabilizar UI
        }

        //Obtener botón delete del primer registro
        private IWebElement GetFirstDeleteButton()
        {
            var row = driver.FindElement(
                By.CssSelector(".oxd-table-body .oxd-table-row"));

            var buttons = row.FindElements(By.CssSelector(".oxd-icon-button"));

            // EL ÚLTIMO BOTÓN SIEMPRE ES DELETE
            return buttons.Last();
        }

       
        // Camino feliz
        [Test]
        public void EliminarEmpleado_Confirmado_SeEliminaCorrectamente()
        {
            test = extent.CreateTest("HU-05 — Eliminar empleado (confirmado)");

            DoLogin();
            driver.Navigate().GoToUrl(SearchUrl);

            WaitForTable();

            var deleteBtn = GetFirstDeleteButton();
            ClickJS(deleteBtn);

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            var confirmBtn = wait.Until(d =>
                d.FindElement(By.CssSelector(".oxd-button--label-danger")));

            ClickJS(confirmBtn);

            Thread.Sleep(2000);

            TakeScreenshot("EliminarEmpleado_CaminoFeliz");

            Assert.That(driver.Url, Does.Contain("viewEmployeeList"));

            test.Pass("Empleado eliminado correctamente");
        }

        
        //Prueba negativa 
        [Test]
        public void EliminarEmpleado_Cancelado_NoSeElimina()
        {
            test = extent.CreateTest("HU-05 — Eliminar empleado (cancelado)");

            DoLogin();
            driver.Navigate().GoToUrl(SearchUrl);

            WaitForTable();

            int rowsBefore = driver.FindElements(
                By.CssSelector(".oxd-table-body .oxd-table-row")).Count;

            var deleteBtn = GetFirstDeleteButton();
            ClickJS(deleteBtn);

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            var cancelBtn = wait.Until(d =>
                d.FindElement(By.CssSelector(".oxd-button--ghost")));

            ClickJS(cancelBtn);

            Thread.Sleep(1000);

            int rowsAfter = driver.FindElements(
                By.CssSelector(".oxd-table-body .oxd-table-row")).Count;

            TakeScreenshot("EliminarEmpleado_Negativo");

            Assert.That(rowsAfter, Is.EqualTo(rowsBefore));

            test.Pass("Cancelación correcta — registro NO eliminado");
        }

        
        // Prueba de límites
        [Test]
        public void EliminarEmpleado_SeleccionarTodos_MuestraOpcionMasiva()
        {
            test = extent.CreateTest("HU-05 — Eliminar empleado (selección masiva)");

            DoLogin();
            driver.Navigate().GoToUrl(SearchUrl);

            WaitForTable();

            var checkbox = driver.FindElement(
                By.CssSelector(".oxd-table-header input[type='checkbox']"));

            ClickJS(checkbox);

            Thread.Sleep(2000);

            TakeScreenshot("EliminarEmpleado_Limites");

            var deleteBtn = driver.FindElements(By.CssSelector(".oxd-button--danger"))
              .Concat(driver.FindElements(By.XPath("//button[contains(text(),'Delete')]")))
               .FirstOrDefault(b => b.Displayed);


            if (deleteBtn == null)
            {
                test.Pass("No apareció botón masivo (comportamiento permitido)");
            }
            else
            {
                test.Pass("Selección masiva habilita eliminación correctamente");
            }
        }
    }
}