namespace Six.Test.Selenium.Extension.WebDriver.WebElements;

public abstract class Input : WebElement
{
    /// <inheritdoc />
    protected Input(OpenQA.Selenium.IWebElement webElement, string tagName) : base(webElement)
    {
        CheckTagName(tagName);
    }

    protected void CheckType(string type)
    {
        var elementType = TheWebElement.GetAttribute("type");
        if (!elementType.Equals(type))
        {
            throw new WrongInputTypeException(type, elementType);
        }
    }

    private void CheckTagName(string tageName)
    {
        var elementTagName = TheWebElement.TagName;
        if (!elementTagName.Equals(tageName))
        {
            throw new WrongTagNameException(tageName, elementTagName);
        }
    }
}