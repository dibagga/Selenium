using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using OpenQA.Selenium;

namespace Six.Test.Selenium.Extension.WebDriver.WebElements;

public class SelectElement : Input
{
    private const int OptionsLimiter = 10;
    private static readonly ILog Logger = LogManager.GetLogger(typeof(SelectElement));
    public readonly OpenQA.Selenium.Support.UI.SelectElement WebElement;

    public SelectElement(OpenQA.Selenium.IWebElement webElement) : base(webElement, "select")
    {
        WebElement = new OpenQA.Selenium.Support.UI.SelectElement(webElement);
    }

    /// <summary>
    ///     returns the texts of all possible options
    /// </summary>
    public IEnumerable<string> Options
    {
        get { return WebElement.Options.Select(s => s.Text); }
    }

    /// <summary>
    ///     returns the text of the selected option
    /// </summary>
    public string SelectedOption => WebElement.SelectedOption.Text;

    /// <summary>
    ///     Selects an option with the given text, which is visible to the user. If it can't find the text in the drop-down
    ///     list it tries to select an entry containing the string.
    /// </summary>
    /// <param name="text">the text of a select option</param>
    public void SelectByText(string text)
    {
        try
        {
            SelectTolerant(text);
        }
        catch (WebDriverException e)
        {
            Logger.Warn(e.Message);
            HandleObscuredElementExceptionByScrollingToElement(e);
            SelectTolerant(text);
            Logger.Debug($"SelectByText performed successfully after retry because of [{e}]");
        }
    }

    /// <summary>
    ///     Selects an option with the given value, which is usually not visible to the user
    /// </summary>
    /// <param name="value">the (internal) value of a select option</param>
    public void SelectByValue(string value)
    {
        try
        {
            WebElement.SelectByValue(value);
        }
        catch (WebDriverException e)
        {
            Logger.Warn(e.Message);
            HandleObscuredElementExceptionByScrollingToElement(e);
            WebElement.SelectByValue(value);
            Logger.Debug($"SelectByValue performed successfully after retry because of [{e}]");
        }
    }

    private void SelectTolerant(string text)
    {
        try
        {
            WebElement.SelectByText(text);
        }
        catch (NoSuchElementException)
        {
            Logger.Debug($"select from by text containing [{text}]");
            var firstContainingElement = WebElement.Options.FirstOrDefault(o => o.Text.IndexOf(text, StringComparison.OrdinalIgnoreCase) >= 0);
            if (firstContainingElement != null)
            {
                firstContainingElement.Click();
            }
            else
            {
                Logger.Warn($"[{text}] not found in list");
                Logger.Info($"Values found [{string.Join("|", WebElement.Options.Select(e => e.Text).Take(OptionsLimiter))}]");
                throw;
            }
        }
    }
}