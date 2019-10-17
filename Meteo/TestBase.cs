using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.Extensions;
using System;
using System.IO;
using System.Reflection;

namespace Meto
{
    /*----------------------------------------------------------------------------------------------------*/
    /// <author>Justinas Abramavicius</author>                                           <date>2019 10</date>
    /// <summary>
    /// Base class to handle common tests setup
    /// </summary>
    /*--------------+---------------+---------------+---------------+---------------+---------------+------*/
    public class TestBase
    {
        private static string _screenShotsDirectory;
        protected IWebDriver Driver;
        protected string ScreenShotPath;
        protected ExtentReports Extent;
        protected ExtentTest Test;

        public static string ReportPath { get; private set; }

        /*----------------------------------------------------------------------------------------------------*/
        /// <author>Justinas Abramavicius</author>                                           <date>2019 10</date>
        /// <summary>
        /// Initialize html reporter 
        /// </summary>
        /*--------------+---------------+---------------+---------------+---------------+---------------+------*/
        [OneTimeSetUp]
        protected void BeforeClass()
        {
            Extent = new ExtentReports();
            var dir = GetDirectory() + @"\Reports";
            Directory.CreateDirectory(dir);
            ReportPath = dir + @"\index.html";
            var htmlReporter = new ExtentHtmlReporter(ReportPath);
            Extent.AddSystemInfo("Driver", "Chrome web driver");
            Extent.AddSystemInfo("System under test", "meteo.lt");
            Extent.AttachReporter(htmlReporter);
        }

        [OneTimeTearDown]
        protected void AfterClass()
        {
            Extent.Flush();
        }

        /*----------------------------------------------------------------------------------------------------*/
        /// <author>Justinas Abramavicius</author>                                           <date>2019 10</date>
        /// <summary>
        /// Captures screen-shot then assertion fails 
        /// </summary>
        /*--------------+---------------+---------------+---------------+---------------+---------------+------*/
        protected void ScreenShotOnFail(Action action)
        {
            try
            {
                action();
            }
            catch (Exception)
            {
                if (_screenShotsDirectory == null)
                {
                    _screenShotsDirectory = GetDirectory() + @"\ScreenShots\";
                    Directory.CreateDirectory(_screenShotsDirectory);
                }

                var screenshot = Driver.TakeScreenshot();
                var fileName = TestContext.CurrentContext.Test.MethodName + "_" +
                    DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss") + ".png";
                ScreenShotPath = new Uri(_screenShotsDirectory + fileName).LocalPath;
                screenshot.SaveAsFile(ScreenShotPath, ScreenshotImageFormat.Png);

                throw;
            }
        }

        protected string GetDirectory()
        {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }

        /*----------------------------------------------------------------------------------------------------*/
        /// <author>Justinas Abramavicius</author>                                           <date>2019 10</date>
        /// <summary>
        /// Reports test outcome to html reporter
        /// </summary>
        /*--------------+---------------+---------------+---------------+---------------+---------------+------*/
        protected void ReportTestOutcome()
        {
            var status = TestContext.CurrentContext.Result.Outcome.Status;
            var errorMessage = TestContext.CurrentContext.Result.Message;
            Status logstatus;
            string outcome = "Test outcome - ";
            switch (status)
            {
                case TestStatus.Failed:
                    logstatus = Status.Fail;
                    string screenShotPath = new Uri(ScreenShotPath).LocalPath;
                    Test.Log(logstatus, outcome + logstatus + " – " + errorMessage);
                    Test.Log(logstatus, "Snapshot below: " + Test.AddScreenCaptureFromPath(screenShotPath));
                    break;
                case TestStatus.Skipped:
                    logstatus = Status.Skip;
                    Test.Log(logstatus, outcome + logstatus);
                    break;
                default:
                    logstatus = Status.Pass;
                    Test.Log(logstatus, outcome + logstatus);
                    break;
            }
        }

        protected void CreateTestForReport()
        {
            Test = Extent.CreateTest(TestContext.CurrentContext.Test.Name);
        } 

    }
}
