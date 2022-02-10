using System;
using OpenQA.Selenium;

namespace Six.Test.Selenium.Extension.WebDriver.WebElements;

public abstract class NavigationWebElement : WebElement
{
    protected NavigationWebElement(OpenQA.Selenium.IWebElement webElement) : base(webElement)
    {
    }

    public void ClickAndWaitForUrlChange()
    {
        var originUrl = TestDirector.WebDriver.Url;
        var elementTextOrId = Text ?? GetAttribute("id");

        Click();

        try
        {
            TestDirector.WebDriver.Wait().Until(d => !d.Url.Equals(originUrl, StringComparison.OrdinalIgnoreCase));
        }
        catch (WebDriverTimeoutException e)
        {
            var message =
                $"The {nameof(ClickAndWaitForUrlChange)} on '{elementTextOrId}' in page '{originUrl}' unexpectedly did not change url (not at all or not in time).";
            throw new WebDriverTimeoutException(message, e);
        }
    }
}