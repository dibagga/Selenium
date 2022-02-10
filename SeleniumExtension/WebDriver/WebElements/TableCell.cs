namespace Six.Test.Selenium.Extension.WebDriver.WebElements;

/// <summary>
///     Wraps an IWebElement into a user-friendly table-data-element
/// </summary>
public class TableCell
{
    public readonly IWebElement WebElement;

    /// <summary>
    ///     Creates a table-webElement-element object
    /// </summary>
    /// <param name="webElement">The IWebElement that is wrapped</param>
    public TableCell(OpenQA.Selenium.IWebElement webElement)
    {
        WebElement = new WebElement(webElement);
    }

    public string Text => WebElement.Text;
}