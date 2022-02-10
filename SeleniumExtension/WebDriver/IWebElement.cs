using Six.Test.Selenium.Extension.WebDriver.WebElements;

namespace Six.Test.Selenium.Extension.WebDriver;

/// <summary>
///     Defines the additional methods that are necessary for a common WebElement
/// </summary>
public interface IWebElement : OpenQA.Selenium.IWebElement
{
    ButtonElement Button();
    CheckBoxElement CheckBox();
    DatePickerElement DatePickerWrappedField();
    DateTimePickerElement DateTimePickerWrappedField();
    LinkElement Link();
    TextFieldElement PasswordTextField();
    RadioButtonElement RadioButton(string radioGroupName);
    SelectElement Selector();
    Table Table();
    TextAreaElement TextArea();
    TextFieldElement TextField(string type = "text");
}