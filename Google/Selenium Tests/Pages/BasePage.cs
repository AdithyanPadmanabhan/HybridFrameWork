using AventStack.ExtentReports.Reporter;
using AventStack.ExtentReports;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;

namespace Google.Selenium_Tests.Pages
{
    public class BasePage
    {
        protected IWebDriver? Driver;
        protected ExtentReports? Extent;
        protected ExtentTest? Test;

        private ExtentSparkReporter? SparkReporter;
        private Dictionary<string, string>? Properties;

        protected string? currentDirectory;
        protected string? url;

        //overloaded constructor

        protected BasePage()
        {

            currentDirectory = Directory.GetParent(@"../../../")?.FullName;
        }

        public BasePage(IWebDriver driver)
        {
            Driver = driver;
        }


        public void ReadConfigSettings()

        {

            Properties = new Dictionary<string, string>();
            string fileName = currentDirectory + "/ConfigSettings/config.properties";
            string[] lines = File.ReadAllLines(fileName);

            foreach (string line in lines)
            {
                if (!string.IsNullOrWhiteSpace(line) && line.Contains('='))
                {
                    string[] parts = line.Split('=');
                    string key = parts[0].Trim();
                    string value = parts[1].Trim();
                    Properties[key] = value;
                }
            }

        }
        protected string TakeScreenShot()
        {
            ITakesScreenshot? its = (ITakesScreenshot?)Driver;
            Screenshot? ss = its?.GetScreenshot();

            string filePath = currentDirectory + "/Screenshots/ss_" + DateTime.Now.ToString("yyyy-mm-dd_HH.mm.ss") + ".png";
            ss?.SaveAsFile(filePath);

            return filePath;
          

        }
        protected static void ScrollIntoView(IWebDriver driver, IWebElement element)
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            js.ExecuteScript("arguments[0].scrollIntoView(true)", element);
        }
        protected void LogTestResult(string testName, string type,ExtentTest Test, string result, string? errorMessage = null)
        {


            if (type.ToLower().Equals("info"))
            {
                Log.Information(result);
                Test?.Info(result);
            }
            else if (type.ToLower().Equals("pass") && errorMessage == null)
            {
                Log.Information(testName + "Passed");
                Log.Information("---------------------------------------------------------------");

                Test?.Pass(result);

            }
            else
            {
                var screenshotpath = TakeScreenShot();
                Log.Error($"Test failed for {testName}.\n Exception: \n{errorMessage}");
                Log.Information("---------------------------------------------------------------");

             
                Test?.AddScreenCaptureFromBase64String(screenshotpath,testName);
                Test?.Fail(result);
            }
        }
        protected void InitializeDriver()
        {

            Extent = new ExtentReports();
            SparkReporter = new ExtentSparkReporter(currentDirectory + "/ExtentReports/extent-report"
                + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".html");

            Extent.AttachReporter(SparkReporter);
            ReadConfigSettings();
            if (Properties?["browser"].ToLower() == "chrome")
            {
                Driver = new ChromeDriver();

            }
            else if (Properties?["browser"].ToLower() == "edge")
            {
                Driver = new EdgeDriver();

            }
            url = Properties?["baseUrl"];
            Driver.Url = url;
            Driver.Manage().Window.Maximize();
        }

        [OneTimeSetUp]
        public void Setup()
        {

            InitializeDriver();
            //config Serilog
            string logfilePath = currentDirectory + "/Logs/log_" + DateTime.Now.ToString("yyyy-mm-dd_HH.mm.ss") + ".txt";
            Log.Logger = new LoggerConfiguration()
               .WriteTo.Console()
               .WriteTo.File(logfilePath, rollingInterval: RollingInterval.Day).CreateLogger();



            //Configure Extent Report

            Extent = new ExtentReports();
            SparkReporter = new ExtentSparkReporter(currentDirectory + "/Reports/report-"
                + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".html");

            Extent.AttachReporter(SparkReporter);
           

        }

        [OneTimeTearDown]
        public void TearDown()
        {
            Driver?.Quit();
            Extent?.Flush();
            Log.CloseAndFlush();


        }


        protected static void WaitForElementToBeClickable(IWebElement? element,string elementName)
        {


            if (element != null)
            {
                DefaultWait<IWebElement> fluentWait = new DefaultWait<IWebElement>(element);
                fluentWait.Timeout = TimeSpan.FromSeconds(5);
                fluentWait.PollingInterval = TimeSpan.FromMilliseconds(150);
                fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException));
                fluentWait.Message = "Element - " + elementName + " - not found or " + "not clickable";
                fluentWait.Until(x => x   != null && x.Displayed && x.Enabled)
;           }
        }
    }
}
