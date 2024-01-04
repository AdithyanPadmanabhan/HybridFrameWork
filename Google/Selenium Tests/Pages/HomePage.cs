using AventStack.ExtentReports;
using Google.Custom_Exceptions;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.PageObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Google.Selenium_Tests.Pages
{
    internal class HomePage : BasePage
    {
        [CacheLookup]
        private IWebElement? SearchBox => Driver?.FindElement(By.Name("q"));
       
        [CacheLookup]
       

        private IWebElement? SearchButton => Driver?.FindElement(By.Name("btnK"));

        public HomePage(IWebDriver driver) : base(driver) { }


      /*  public void EnterSearchText(string? searchText)
        {
            SearchBox?.SendKeys(searchText);
        }

        public void ClickSearchButton()
        {
            DefaultWait<IWebDriver> fluentWait = new DefaultWait<IWebDriver>(Driver);
            fluentWait.Timeout = TimeSpan.FromSeconds(5);
            fluentWait.PollingInterval = TimeSpan.FromMilliseconds(250);
            fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException));
            fluentWait.Message = "Element not found";
            fluentWait.Until(x => x.FindElement(By.Name("btnK")));
            SearchButton?.Click();
        }*/

        public void SearchTest(string searchText,string testName, ExtentTest Test)
        {

            try
            {
                SearchBox?.SendKeys(searchText);
                LogTestResult(testName, "Info", Test,$"Entered search text: {searchText}");
                WaitForElementToBeClickable(SearchButton, "Google Search Button");
                SearchButton?.Click();
                LogTestResult(testName, "Info", Test, "Clicked on the Search button");

            }

            catch(Exception ex)
            {
                throw new ProjectExceptions(ex.Message);
            }
        }

    }
}
