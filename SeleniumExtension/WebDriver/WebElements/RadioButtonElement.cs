using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;

namespace Six.Test.Selenium.Extension.WebDriver.WebElements;

public class RadioButtonElement : Input
{
    private const string InputType = "radio";
    private const string RadioTagName = "input";
    private readonly IEnumerable<OpenQA.Selenium.IWebElement> _radioGroup;

    /// <summary>
    ///     Finds all radio buttons within a web element under a given name
    /// </summary>
    /// <param name="webElement"></param>
    /// <param name="name"></param>
    public RadioButtonElement(OpenQA.Selenium.IWebElement webElement, string name) : base(webElement, RadioTagName)
    {
        CheckType(InputType);

        _radioGroup = TestDirector.WebDriver.FindElements(By.CssSelector($"input[type='{InputType}'][name='{name}']"));
    }

    public List<OpenQA.Selenium.IWebElement> SelectedItems => _radioGroup.Where(e => e.Selected).ToList();

    /// <summary>
    ///     Returns whether the radio button is selected
    /// </summary>
    /// <returns>true if the element is selected, else false</returns>
    public bool IsSelected(string value)
    {
        return _radioGroup.First(e => e.GetAttribute("value").Equals(value)).Selected;
    }

    /// <summary>
    ///     clicks on a radio button which matches the (first) value
    /// </summary>
    /// <param name="value">the radio button value to be selected</param>
    public void Select(string value)
    {
        _radioGroup.First(e => e.GetAttribute("value").Equals(value)).Click(); //..make equivalent
    }
}