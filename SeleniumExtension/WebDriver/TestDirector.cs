using System;
using System.Net;
using log4net;
using OpenQA.Selenium;
using SIX.SCS.Tools.Helper;
using Six.Test.Selenium.Extension.Browser;
using Six.Test.Selenium.Extension.Environment;

namespace Six.Test.Selenium.Extension.WebDriver;

/// <summary>
///     Setup test environment and create a browser instance. Provides a start/stop- and login/logoff mechanism.
/// </summary>
public static class TestDirector
{
    private const string HomePathUrl = "";
    private const int MaxAttempts = 10;
    private static readonly ILog Logger = LogManager.GetLogger(typeof(TestDirector));
    public static TestEnvironment TestEnvironment { get; set; }
    public static IWebDriver WebDriver { get; private set; }

    /// <summary>
    ///     Navigates to the url if not already there
    /// </summary>
    /// <param name="urlSuffix">the url to navigate to</param>
    /// <param name="forcedNavigation">enforces navigation even if the current url is the same</param>
    public static void Navigate(string urlSuffix = HomePathUrl, bool forcedNavigation = false)
    {
        var suffix = new Uri(urlSuffix, UriKind.Relative);
        var url = new Uri(TestEnvironment.BaseUrl, suffix);
        if (forcedNavigation || WebDriver.Url != url.AbsoluteUri)
        {
            Logger.Info($"Navigating -> [{url.AbsoluteUri}]");
            WebDriver.Url = url.AbsoluteUri;
        }
        else
        {
            Logger.Info($"No Navigating = [{url.AbsoluteUri}]");
        }
    }

    public static void Refresh()
    {
        Logger.Info("Refreshing Browser");
        WebDriver.Navigate().Refresh();
    }

    /// <summary>
    ///     Execute the required authentication procedure to fulfill the basic precondition of testing.
    /// </summary>
    public static void Start()
    {
        PrepareBrowser();

        Retry.Until(
            () =>
            {
                Navigate();
                TestEnvironment.Authentication.LogOn();
                return true;
            },
            () => WebDriver.TakeScreenShot(),
            "Logon attempt failed",
            MaxAttempts);
        Logger.Info("Logged on");
    }

    public static void Stop()
    {
        try
        {
            TestEnvironment.Application.LogOff();
        }
        catch (Exception e)
        {
            Logger.Error($"Logoff failed because: {e.Message}");
        }

        try
        {
            WebDriver.Quit();
        }
        catch (Exception e)
        {
            Logger.Error($"Closing WebDriver failed because: {e.Message}");
            throw;
        }
    }

    /// <summary>
    ///     If the address is set, the Selenium Hub specified is used. If the address is null or empty the execution is done
    ///     locally
    /// </summary>
    /// <returns></returns>
    private static void PrepareBrowser()
    {
        string runOn;

        if (string.IsNullOrEmpty(TestEnvironment.SeleniumConfig.GridAddress) || TestEnvironment.ForceLocal)
        {
            runOn = "LOCAL";
            try
            {
                WebDriver = BrowserFactory.GetBrowserFactory(TestEnvironment).Local();
            }
            catch (Exception e)
            {
                Logger.Error($"Failed to prepare {runOn} WebDriver instance [{e.Message}]");
                throw;
            }
        }
        else
        {
            runOn = $"GRID [{TestEnvironment.SeleniumConfig.GridAddress}]";
            try
            {
                WebDriver = new WebDriver(BrowserFactory.GetBrowserFactory(TestEnvironment).Remote());
            }
            catch (WebDriverException e)
            {
                Logger.Error($"Failed to prepare {runOn} WebDriver instance [{e.Message}] (check if GRID Server and Node are running properly)");
                throw;
            }
        }

        Logger.Info($"Launched browser using Selenium {runOn} on machine [{Dns.GetHostName()}]");
    }
}