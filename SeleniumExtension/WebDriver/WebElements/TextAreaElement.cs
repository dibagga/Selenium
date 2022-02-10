using System.Collections.Generic;
using System.Linq;

namespace Six.Test.Selenium.Extension.WebDriver.WebElements;

/// <summary>
///     A multiline text-field
/// </summary>
public class TextAreaElement : TextFieldElement
{
    private const char Separator = '\n';

    public TextAreaElement(OpenQA.Selenium.IWebElement webElement) : base(webElement, "textarea")
    {
    }

    public IEnumerable<string> Lines()
    {
        return Text.Split(Separator);
    }

    public void TypeText(IEnumerable<string> lines)
    {
        var linesAsOneString = lines.Aggregate(
            "",
            (sourceString, concatenatedString) => string.Concat(sourceString, string.Concat(concatenatedString, Separator)));

        TypeText(linesAsOneString.TrimEnd('\n'));
    }
}