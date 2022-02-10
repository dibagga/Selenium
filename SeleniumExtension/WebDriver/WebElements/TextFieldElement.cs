using log4net;
using OpenQA.Selenium;

namespace Six.Test.Selenium.Extension.WebDriver.WebElements;

public class TextFieldElement : Input
{
    private static readonly ILog Logger = LogManager.GetLogger(typeof(SelectElement));

    public TextFieldElement(OpenQA.Selenium.IWebElement webElement, string tag = "input", string type = "text") : base(webElement, tag)
    {
        CheckType(type);
    }

    protected TextFieldElement(OpenQA.Selenium.IWebElement webElement, string tag) : base(webElement, tag)
    {
    }

    /// <summary>
    ///     Reads the text from the text-field
    /// </summary>
    /// <returns>the string value from the text-field</returns>
    public override string Text
    {
        get
        {
            try
            {
                return TheWebElement.GetAttribute("value");
            }
            catch (WebDriverException)
            {
                TestDirector.WebDriver.TakeScreenShot();
                throw;
            }
        }
    }

    /// <summary>
    ///     Types in the given string into the textfield by first clearing it.
    /// </summary>
    /// <param name="userInput"></param>
    public void TypeText(string userInput)
    {
        try
        {
            TheWebElement.Clear();
            SendKeys(userInput);
        }
        catch (WebDriverException e)
        {
            Logger.Warn(e.Message);
            TestDirector.WebDriver.TakeScreenShot();
            throw;
        }
    }
}