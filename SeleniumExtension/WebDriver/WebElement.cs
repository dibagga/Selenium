using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using log4net;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.Extensions;
using SeleniumExtras.WaitHelpers;
using Six.Test.Selenium.Extension.WebDriver.WebElements;

namespace Six.Test.Selenium.Extension.WebDriver;

/// <summary>
///     This class wraps WebElements from selenium and adds features which are commonly needed.
/// </summary>
public class WebElement : IWebElement
{
    private const int NavBarPadding = 15;
    private static readonly ILog Logger = LogManager.GetLogger(typeof(WebElement));
    protected readonly OpenQA.Selenium.IWebElement TheWebElement;

    public WebElement(OpenQA.Selenium.IWebElement webElement)
    {
        TheWebElement = webElement;
    }

    public virtual bool Displayed
    {
        get
        {
            try
            {
                return TheWebElement.Displayed;
            }
            catch (WebDriverException e)
            {
                Logger.Warn(e.Message);
                TestDirector.WebDriver.TakeScreenShot();
                throw;
            }
        }
    }

    public bool Enabled
    {
        get
        {
            try
            {
                return TheWebElement.Enabled;
            }
            catch (WebDriverException e)
            {
                Logger.Warn(e.Message);
                TestDirector.WebDriver.TakeScreenShot();
                throw;
            }
        }
    }

    public Point Location => TheWebElement.Location;

    public bool Selected
    {
        get
        {
            try
            {
                return TheWebElement.Selected;
            }
            catch (WebDriverException e)
            {
                Logger.Warn(e.Message);
                TestDirector.WebDriver.TakeScreenShot();
                throw;
            }
        }
    }

    public Size Size => TheWebElement.Size;

    /// <summary>
    ///     Gets the tag name of this element.
    /// </summary>
    public string TagName => TheWebElement.TagName;

    public virtual string Text
    {
        get
        {
            try
            {
                return TheWebElement.Text;
            }
            catch (StaleElementReferenceException e)
            {
                Logger.Warn(e.Message);
                var w = TestDirector.WebDriver.Wait();
                w.Timeout = TimeSpan.FromSeconds(10);
                w.Message = "StaleException Text";
                w.IgnoreExceptionTypes(typeof(StaleElementReferenceException));
                w.Until(_ => TheWebElement.Text != null);
                Logger.Debug("StaleException");
                return TheWebElement.Text;
            }
            catch (WebDriverException e)
            {
                TestDirector.WebDriver.TakeScreenShot();
                Logger.Warn(e.Message);
                throw;
            }
        }
    }

    public ButtonElement Button()
    {
        return new ButtonElement(TheWebElement);
    }

    public CheckBoxElement CheckBox()
    {
        return new CheckBoxElement(TheWebElement);
    }

    public void Clear()
    {
        TheWebElement.Clear();
    }

    public void Click()
    {
        try
        {
            TheWebElement.Click();
        }
        catch (StaleElementReferenceException e)
        {
            Logger.Debug($"StaleElementReferenceException [{e.Message}]");
            throw;
        }
        catch (WebDriverException e)
        {
            Logger.Warn(e.Message);
            HandleObscuredElementExceptionByScrollingToElement(e);
            TheWebElement.Click();
            Logger.Debug($"Click performed successfully after retry because of [{e}]");
        }
    }

    public DatePickerElement DatePickerWrappedField()
    {
        return new DatePickerElement(TheWebElement);
    }

    public DateTimePickerElement DateTimePickerWrappedField()
    {
        return new DateTimePickerElement(TheWebElement);
    }

    public OpenQA.Selenium.IWebElement FindElement(By by)
    {
        return WebDriver.FindElementWithRetry(by, TheWebElement);
    }

    public ReadOnlyCollection<OpenQA.Selenium.IWebElement> FindElements(By by)
    {
        var webElements = TheWebElement.FindElements(by);
        if (webElements.Count == 0)
        {
            Logger.Debug($"FindElements by [{by}] contains no elements");
        }

        return webElements;
    }

    public string GetAttribute(string attributeName)
    {
        return TheWebElement.GetAttribute(attributeName);
    }

    public string GetCssValue(string propertyName)
    {
        return TheWebElement.GetCssValue(propertyName);
    }

    /// <inheritdoc />
    public string GetDomAttribute(string attributeName)
    {
        return TheWebElement.GetDomAttribute(attributeName);
    }

    /// <inheritdoc />
    public string GetDomProperty(string propertyName)
    {
        return TheWebElement.GetDomProperty(propertyName);
    }

    public string GetProperty(string propertyName)
    {
        throw new NotSupportedException("obsolete code");
    }

    /// <inheritdoc />
    public ISearchContext GetShadowRoot()
    {
        return TheWebElement.GetShadowRoot();
    }

    public LinkElement Link()
    {
        return new LinkElement(TheWebElement);
    }

    public TextFieldElement PasswordTextField()
    {
        return new TextFieldElement(TheWebElement, type: "password");
    }

    public RadioButtonElement RadioButton(string option)
    {
        return new RadioButtonElement(TheWebElement, option);
    }

    /// <summary>
    ///     Adds method for drop down lists it returns a new reference of a 'SelectElement' on which different selects (by
    ///     value, name etc.)
    ///     can be executed
    /// </summary>
    /// <returns>SelectElement for the actual WebElement</returns>
    public SelectElement Selector()
    {
        return new SelectElement(TheWebElement);
    }

    public void SendKeys(string text)
    {
        try
        {
            TheWebElement.SendKeys(text);
        }
        catch (WebDriverException e)
        {
            Logger.Warn(e.Message);
            HandleObscuredElementExceptionByScrollingToElement(e);
            TheWebElement.SendKeys(text);
            Logger.Debug($"SendKeys performed successfully after retry because of [{e}]");
        }
    }

    public void Submit()
    {
        TheWebElement.Submit();
    }

    public Table Table()
    {
        return new Table(TheWebElement);
    }

    public TextAreaElement TextArea()
    {
        return new TextAreaElement(TheWebElement);
    }

    public TextFieldElement TextField(string type = "text")
    {
        return new TextFieldElement(TheWebElement, type: type);
    }

    protected void HandleObscuredElementExceptionByScrollingToElement(WebDriverException exception)
    {
        TestDirector.WebDriver.TakeScreenShot();

        if (exception is not ElementClickInterceptedException and not ElementNotVisibleException and not ElementNotInteractableException)
        {
            Logger.Debug($"no handling for [{TheWebElement}] because of [{exception.GetBaseException()}]");
            throw exception;
        }

        Logger.Info(exception.Message);
        Logger.Debug(exception.GetBaseException());

        ScrollToElement();
    }

    private static int GetOverlayHeight()
    {
        var overlays = TestDirector.WebDriver.FindElements(By.CssSelector(".sticky-top, .sticky-summary, [id='DashboardSummaryContainer']"));
        var sum = overlays.Sum(e => e.Size.Height) + NavBarPadding;
        Logger.Debug($"Number of overlay elements [{overlays.Count}] with sum [{sum}]");
        return sum;
    }

    private void ScrollToElement()
    {
        var overlayHeight = GetOverlayHeight();
        var x = TheWebElement.Location.X;
        var y = Math.Max(0, TheWebElement.Location.Y - overlayHeight);

        try
        {
            Logger.Debug($"scrolling to x[{x}] y[{y}] to make element [{TheWebElement.TagName}] interactable");
            TestDirector.WebDriver.ExecuteJavaScript($"window.scrollTo({x}, {y})");
        }
        catch (Exception e)
        {
            Logger.Warn($"failed to scroll to [{TheWebElement}] with coordination x[{x}] y[{y}] because [{e.Message}]");
        }

        WaitUntilClickable();
    }

    private void WaitUntilClickable()
    {
        try
        {
            TestDirector.WebDriver.Wait(TimeSpan.FromSeconds(5)).Until(ExpectedConditions.ElementToBeClickable(TheWebElement));
        }
        catch (WebDriverException e)
        {
            Logger.Debug($"Wait until element clickable [{TheWebElement.TagName}]: {e.Message}");
            TestDirector.WebDriver.TakeScreenShot();
            throw;
        }
    }
}