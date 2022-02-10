using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using log4net;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.Events;
using OpenQA.Selenium.Support.UI;

namespace Six.Test.Selenium.Extension.WebDriver;

/// <summary>
///     This WebDriver adapter implements additional features to the Selenium provided.
/// </summary>
public class WebDriver : IWebDriver
{
    private const int MaxAttempts = 11;
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(55);
    private static readonly ILog Logger = LogManager.GetLogger(typeof(WebDriver));

    public WebDriver(OpenQA.Selenium.IWebDriver webDriver)
    {
        WrappedWebDriver = new EventFiringWebDriver(webDriver);

        AddEventHandlers();
        JsErrorHandler = new JsErrorHandler(this);
        ScreenshotHelper = new ScreenshotHelper(this);
    }

    public Actions Actions => new(WrappedWebDriver);
    public string CurrentWindowHandle => WrappedWebDriver.CurrentWindowHandle;

    public string PageSource => WrappedWebDriver.PageSource;

    public string Title => WrappedWebDriver.Title;

    public string Url
    {
        get => WrappedWebDriver.Url;
        set
        {
            try
            {
                WrappedWebDriver.Url = value;
            }
            catch (WebDriverException e)
            {
                TakeScreenShot();
                Logger.Error($"Failed navigate to {value} [{e.Message}]");
                throw;
            }
        }
    }

    public ReadOnlyCollection<string> WindowHandles => WrappedWebDriver.WindowHandles;
    internal EventFiringWebDriver WrappedWebDriver { get; }

    private JsErrorHandler JsErrorHandler { get; }

    private ScreenshotHelper ScreenshotHelper { get; }

    public static OpenQA.Selenium.IWebElement FindElementWithRetry(By by, ISearchContext searchContext)
    {
        for (var attempts = MaxAttempts; attempts > 0; attempts--)
        {
            try
            {
                return searchContext.FindElement(by);
            }
            catch (NoSuchElementException e)
            {
                Logger.Warn($"FindElement by [{by}] threw [{e.Message}]");

                TestDirector.WebDriver.TakeScreenShot();
                TestDirector.WebDriver.WriteAnyDisplayedPageErrors(by);
                throw;
            }
            catch (NotFoundException e)
            {
                Logger.Warn($"FindElement by [{by}] threw [{e.Message}]");

                TestDirector.WebDriver.TakeScreenShot();
                TestDirector.WebDriver.WriteAnyDisplayedPageErrors(by);
                throw;
            }
            catch (StaleElementReferenceException e)
            {
                Logger.Warn($"FindElement by [{by}] is stale [{e.Message}]");
            }
        }

        Logger.Error($"Failed to FindElement by [{by}]after {MaxAttempts} attempts");
        throw new WebDriverException($"Failed to FindElement By[{by}] after {MaxAttempts} attempts");
    }

    public void Close()
    {
        WrappedWebDriver.Close();
    }

    public void Dispose()
    {
        WrappedWebDriver.Dispose();
    }

    /// <inheritdoc />
    public object ExecuteAsyncScript(string script, params object[] args)
    {
        return WrappedWebDriver.ExecuteScript(script, args);
    }

    /// <inheritdoc />
    public object ExecuteScript(PinnedScript script, params object[] args)
    {
        return WrappedWebDriver.ExecuteScript(script, args);
    }

    /// <inheritdoc />
    public object ExecuteScript(string script, params object[] args)
    {
        return WrappedWebDriver.ExecuteScript(script, args);
    }

    public IWebElement FindAdaptedElement(By by)
    {
        return new WebElement(FindElementWithRetry(by, WrappedWebDriver));
    }

    public IEnumerable<IWebElement> FindAdaptedElements(By by)
    {
        var webElements = WrappedWebDriver.FindElements(by);
        if (webElements.Count == 0)
        {
            Logger.Info($"FindElements by [{by}] contains no elements");
        }

        return webElements.Select(e => new WebElement(e));
    }

    public OpenQA.Selenium.IWebElement FindElement(By by)
    {
        return FindElementWithRetry(by, WrappedWebDriver);
    }

    public ReadOnlyCollection<OpenQA.Selenium.IWebElement> FindElements(By by)
    {
        var webElements = WrappedWebDriver.FindElements(by);
        if (webElements.Count == 0)
        {
            Logger.Debug($"FindElements by [{by}] contains no elements");
        }

        return webElements;
    }

    public IOptions Manage()
    {
        return WrappedWebDriver.Manage();
    }

    public INavigation Navigate()
    {
        return WrappedWebDriver.Navigate();
    }

    public void Quit()
    {
        WrappedWebDriver.Quit();
    }

    public ITargetLocator SwitchTo()
    {
        return WrappedWebDriver.SwitchTo();
    }

    public FileInfo TakeScreenShot(string filePrefix = "")
    {
        return ScreenshotHelper.TakeScreenShot(filePrefix);
    }

    public WebDriverWait Wait(TimeSpan? timeout = null)
    {
        var wait = new WebDriverWait(WrappedWebDriver, timeout ?? DefaultTimeout);
        wait.IgnoreExceptionTypes(typeof(StaleElementReferenceException));
        return wait;
    }

    public void WriteAnyDisplayedPageErrors(By by)
    {
        WrappedWebDriver.FindElements(By.CssSelector("div#content div.formAlert, div#content div.alert"))
            .Select(e => e.Text)
            .ToList()
            .ForEach(
                error =>
                {
                    Logger.Error(error);
                });
        Logger.Warn($"FindElement by [{by}] failed - URL [{WrappedWebDriver.Url}]");
    }

    private void AddEventHandlers()
    {
        WrappedWebDriver.ExceptionThrown += (_, args) =>
        {
            Logger.Warn($"Event:ExceptionThrown [{args.ThrownException.GetType()}: {args.ThrownException.Message}]");
            TakeScreenShot();
        };
        WrappedWebDriver.Navigating += (_, _) =>
        {
            JsErrorHandler.EnsureNoErrors();
        };
        WrappedWebDriver.Navigated += (_, e) =>
        {
            JsErrorHandler.EnsureNoErrors();
            Logger.Info($"Event:Navigated [{e.Url}]");
        };
        WrappedWebDriver.ScriptExecuted += (_, e) =>
        {
            Logger.Info($"Event:ScriptExecuted [{e.Script}] {e}");
        };
        WrappedWebDriver.FindingElement += (_, e) =>
        {
            Logger.Debug($"Event:FindingElement by [{e.FindMethod}]");
        };
        WrappedWebDriver.ElementClicking += (_, e) =>
        {
            Logger.Debug($"Event:ElementClicking [<{new WebElement(e.Element).TagName} text=\"{new WebElement(e.Element).Text}\">]");
        };
    }
}