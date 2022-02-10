using log4net;

namespace Six.Test.Selenium.Extension.WebDriver.WebElements;

public class CheckBoxElement : Input
{
    private static readonly ILog Logger = LogManager.GetLogger(typeof(WebElement));

    public CheckBoxElement(OpenQA.Selenium.IWebElement webElement) : base(webElement, "input")
    {
        CheckType("checkbox");
    }

    /// <summary>
    ///     Returns whether an (input) element is selected or not this
    /// </summary>
    /// <returns>true if the element is selected, else false</returns>
    public bool IsChecked()
    {
        return TheWebElement.Selected;
    }

    /// <summary>
    ///     ensures the de-/activation for a checkbox given to the value and clicks the element (only) if necessary
    /// </summary>
    /// <param name="userInput"></param>
    public void Set(bool userInput)
    {
        Logger.Info($"value for [{TheWebElement.GetAttribute("name")}] is: [{TheWebElement.Selected}] - should be: [{userInput}]");
        if (TheWebElement.Selected ^ userInput)
        {
            Logger.Debug("not equivalent -> click");
            Click();
        }
        else
        {
            Logger.Debug("equivalent -> no click");
        }
    }
}