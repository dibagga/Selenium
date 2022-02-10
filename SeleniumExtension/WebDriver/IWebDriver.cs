using System;
using System.Collections.Generic;
using System.IO;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace Six.Test.Selenium.Extension.WebDriver;

public interface IWebDriver : OpenQA.Selenium.IWebDriver, IJavaScriptExecutor
{
    /// <summary>
    ///     This method returns the internal reference of the RemoteWebDriver to use it e.g. for Action(s) compositions
    /// </summary>
    /// <returns></returns>
    Actions Actions { get; }

    IWebElement FindAdaptedElement(By by);
    IEnumerable<IWebElement> FindAdaptedElements(By by);
    FileInfo TakeScreenShot(string filePrefix = "");
    WebDriverWait Wait(TimeSpan? timeout = null);
    void WriteAnyDisplayedPageErrors(By by);
}