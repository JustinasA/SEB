using Meto;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Linq;

namespace Meteo
{
    /*----------------------------------------------------------------------------------------------------*/
    /// <author>Justinas Abramavicius</author>                                      <date>2019 10</date>
    /// <summary>
    /// Meteo.lt testcases
    /// </summary>
    /*--------------+---------------+---------------+---------------+---------------+---------------+------*/
    [TestFixture]
    public class MeteoTests : TestBase
    {
        private Actions _action;
        private static readonly string _meteoUrl = "http://www.meteo.lt";
        private static readonly string _lietuvosKlimatasUrl = _meteoUrl + "/lt/oro-temperatura";
        private static readonly string _searchElementXPath = ".//form[contains(@class, 'search')]//descendant::input";

        [SetUp]
        public void Setup()
        {
            Driver = new ChromeDriver();
            Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
            Driver.Manage().Window.Maximize();
            _action = new Actions(Driver);
            CreateTestForReport();
        }

        [TearDown]
        public void TearDown()
        {
            ReportTestOutcome();
            Driver.Quit();
        }


        /*----------------------------------------------------------------------------------------------------*/
        /// <author>Justinas Abramavicius</author>                                      <date>2019 10</date>
        /// <type>Positive</type>
        /// <summary>
        /// Navigates to 'Lietuvos klimatas' and checks url for succesfull navigation
        /// </summary>
        /*--------------+---------------+---------------+---------------+---------------+---------------+------*/
        [Test]
        public void TestLietuvosKlimatasReference_PageLoaded()
        {
            Driver.Url = _meteoUrl;
            IWebElement klimatasElement = Driver.FindElementIfExists(By.LinkText("Klimatas"));
            _action.MoveToElement(klimatasElement).Perform();

            IWebElement ltKlimatasElement = klimatasElement.FindElementIfExists(
                By.XPath(".//following-sibling::ul//descendant::a[text()[contains(.,'Lietuvos klimatas')]]"));

            ltKlimatasElement.Click();

            ScreenShotOnFail(() =>
            {
                Assert.AreEqual(_lietuvosKlimatasUrl, Driver.Url, "Lietuvos klimatas page did not load");
            });
        }

        /*----------------------------------------------------------------------------------------------------*/
        /// <author>Justinas Abramavicius</author>                                      <date>2019 10</date>
        /// <type>Positive</type>
        /// <summary>
        /// Checks if Oro Temperatura section exists
        /// </summary>
        /*--------------+---------------+---------------+---------------+---------------+---------------+------*/
        [Test]
        public void TestOroTemperatura_SecsionExists()
        {
            Driver.Url = _lietuvosKlimatasUrl;
            IWebElement oroTemperaturaSectionElement = Driver.FindElementIfExists(
                By.XPath(".//section[contains(@class, 'portlet')]//descendant::h4[text()[contains(.,'Oro temperat')]]"));

            Assert.IsNotNull(oroTemperaturaSectionElement, "Oro Temeratura section does not exists");
        }

        /*----------------------------------------------------------------------------------------------------*/
        /// <author>Justinas Abramavicius</author>                                      <date>2019 10</date>
        /// <type>Positive</type>
        /// <summary>
        /// Searches for existing city. Also fails if load time exceeded.
        /// </summary>
        /*--------------+---------------+---------------+---------------+---------------+---------------+------*/
        [TestCase("Vilnius")]
        [Test]
        public void TestSearch_ResultFound(string searchValue)
        {
            //Load time in ms, took from browser's 'Load' field then manually executing test case
            TimeSpan expectedLoadTimeMs = TimeSpan.FromMilliseconds(4500 * 1.2);
            TimeSpan timeSpan;
            var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(10));
            Driver.Url = _meteoUrl; 
             IWebElement searchElement = Driver.FindElementIfExists(
                By.XPath(_searchElementXPath));

            Assert.IsNotNull(searchElement, "Search input does not exists");

            searchElement.SendKeys(searchValue);
            searchElement.SendKeys(Keys.Enter);

            IWebElement searchResultsExits = wait.Until(Driver => Driver.FindElementIfExists(By.ClassName("dataTables_info")));

            timeSpan = TimeSpan.FromMilliseconds(Driver.DomLoadTimeInMs());

            ScreenShotOnFail(() =>
            {
                Assert.Multiple(() =>
                {
                    Assert.IsNotNull(searchResultsExits, "No search results displayed");
                    Assert.That(timeSpan, Is.LessThan(expectedLoadTimeMs), "Search took too long");
                });
            });
        }

        /*----------------------------------------------------------------------------------------------------*/
        /// <author>Justinas Abramavicius</author>                                      <date>2019 10</date>
        /// <type>Negative</type>
        /// <summary>
        /// Searches for non existing city. Also fails if load time exceeded.
        /// </summary>
        /*--------------+---------------+---------------+---------------+---------------+---------------+------*/
        [TestCase("London")]
        [Test]
        public void TestSearch_ResultNotFound(string searchValue)
        {
            //Load time in ms, took from browser's 'Load' field then manually executing test case
            TimeSpan expectedLoadTimeMs = TimeSpan.FromMilliseconds(3000 * 1.2);
            TimeSpan timeSpan;
            Driver.Url = _meteoUrl;

            IWebElement searchElement = Driver.FindElementIfExists(
                By.XPath(_searchElementXPath));

            Assert.IsNotNull(searchElement, "Search input does not exists");

            searchElement.SendKeys(searchValue);
            searchElement.SendKeys(Keys.Enter);

            IWebElement alertElement = Driver.FindElementIfExists(By.ClassName("alert"));
            IWebElement searchResultsExits = Driver.FindElementIfExists(By.ClassName("dataTables_info"));

            timeSpan = TimeSpan.FromMilliseconds(Driver.DomLoadTimeInMs());

            ScreenShotOnFail(() =>
            {
                Assert.Multiple(() =>
                {
                    Assert.IsNotNull(alertElement, "Alert message does not exists");
                    Assert.IsNull(searchResultsExits, "Shouldn't be any results displayed");
                    Assert.That(timeSpan, Is.LessThan(expectedLoadTimeMs), "Search took too long");
                });
            });
        }

        /*----------------------------------------------------------------------------------------------------*/
        /// <author>Justinas Abramavicius</author>                                      <date>2019 10</date>
        /// <type>Positive</type>
        /// <summary>
        /// One failing test to capture screen-shot to html report
        /// </summary>
        /*--------------+---------------+---------------+---------------+---------------+---------------+------*/
        [Test]
        public void FailingTest_ScreenShotCaptured()
        {
            Driver.Url = _meteoUrl;

            IWebElement forecast = Driver.FindElementIfExists(By.ClassName("forecast"));
            string temperature = Driver.FindElementIfExists(By.XPath(".//descendant::span[contains(@class, 'big')]")).Text;

            ScreenShotOnFail(() =>
            {
                Assert.AreEqual("40 °C", temperature, "No global warming in Lithuania yet!");
            });
        }
    }

    /*----------------------------------------------------------------------------------------------------*/
    /// <author>Justinas Abramavicius</author>                                           <date>2019 10</date>
    /// <summary>
    /// Helper class for extend methods
    /// </summary>
    /*--------------+---------------+---------------+---------------+---------------+---------------+------*/
    internal static class Hellper
    {
        public static IWebElement FindElementIfExists(this IWebDriver driver, By by)
        {
            var elements = driver.FindElements(by);
            return (elements.Count >= 1) ? elements.First() : null;
        }

        public static IWebElement FindElementIfExists(this IWebElement element, By by)
        {
            var elements = element.FindElements(by);
            return (elements.Count >= 1) ? elements.First() : null;
        }

        public static IJavaScriptExecutor Scripts(this IWebDriver driver)
        {
            return (IJavaScriptExecutor)driver;
        }

        /*----------------------------------------------------------------------------------------------------*/
        /// <author>Justinas Abramavicius</author>                                           <date>2019 10</date>
        /// <summary>
        /// Calculates load time. Properties got from browser.
        /// </summary>
        /*--------------+---------------+---------------+---------------+---------------+---------------+------*/
        public static long DomLoadTimeInMs(this IWebDriver driver)
        {
            var navigationStart = (long) driver.Scripts().ExecuteScript("return window.performance.timing.navigationStart");
            var domComplete = (long) driver.Scripts().ExecuteScript("return window.performance.timing.domComplete");
            return domComplete - navigationStart;
        }
    }
}