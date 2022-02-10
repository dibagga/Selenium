namespace Six.Test.Selenium.Extension.WebDriver.WebElements;

public class DatePickerElement : DateTimePickerElement
{
    public DatePickerElement(OpenQA.Selenium.IWebElement webElement) : base(webElement, false)
    {
    }
}