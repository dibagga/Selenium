namespace Six.Test.Selenium.Extension.WebDriver.WebElements;

public class LinkElement : NavigationWebElement
{
    public LinkElement(OpenQA.Selenium.IWebElement webElement) : base(webElement)
    {
    }

    public string Href => GetAttribute("href");
}