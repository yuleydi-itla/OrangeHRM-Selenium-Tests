using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;

namespace OrangeHRM.Tests.Base
{
    public class BaseTest
    {
        protected IWebDriver driver;
        protected static ExtentReports extent;
        protected ExtentTest test;

        private static readonly string ReportPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, "Reports", "TestReport.html");

        [OneTimeSetUp]
        public void InitReport()
        {
            if (extent != null) return; 

            var dir = Path.GetDirectoryName(ReportPath)!;
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            var htmlReporter = new ExtentSparkReporter(ReportPath);
            htmlReporter.Config.DocumentTitle = "OrangeHRM Test Report";
            htmlReporter.Config.ReportName = "Reporte de Pruebas Automatizadas";
            htmlReporter.Config.Theme = AventStack.ExtentReports.Reporter.Config.Theme.Dark;

            extent = new ExtentReports();
            extent.AttachReporter(htmlReporter);

            extent.AddSystemInfo("Aplicación", "OrangeHRM Demo");
            extent.AddSystemInfo("Módulo", "Gestión de Empleados (PIM)");
            extent.AddSystemInfo("Navegador", "Microsoft Edge");
            extent.AddSystemInfo("Framework", "Selenium + NUnit");
            extent.AddSystemInfo("Tipo de pruebas", "Automatizadas UI");
            extent.AddSystemInfo("Tester", "Yuleydi De Los Santos");
        }

        [SetUp]
        public void InitDriver()
        {
            var options = new EdgeOptions();
            var service = EdgeDriverService.CreateDefaultService(@"C:\WebDrivers\");
            driver = new EdgeDriver(service, options);
            driver.Manage().Window.Maximize();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
        }

        [TearDown]
        public void CleanUp()
        {
            if (TestContext.CurrentContext.Result.Outcome.Status
                == NUnit.Framework.Interfaces.TestStatus.Failed)
            {
                TakeScreenshot("FAILED");
            }
            driver?.Quit();
            driver = null;
        }

        [OneTimeTearDown]
        public void FlushReport()
        {
            extent.Flush();
        }

        public void TakeScreenshot(string fileName)
        {
            Screenshot ss = ((ITakesScreenshot)driver).GetScreenshot();

            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "Screenshots");

            // Crear carpeta si no existe
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            string fullPath = Path.Combine(folderPath, fileName + ".png");

            ss.SaveAsFile(fullPath);

            
            test.AddScreenCaptureFromPath(fullPath);
        }
    }
}