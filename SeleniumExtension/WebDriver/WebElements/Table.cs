using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Six.Test.Selenium.Extension.WebDriver.WebElements;

/// <summary>
///     Wraps an IWebElement into a user-friendly table-element
/// </summary>
public class Table
{
    private const string TagName = "table";
    public readonly OpenQA.Selenium.IWebElement WebElement;

    /// <summary>
    ///     Creates a table object
    /// </summary>
    /// <param name="table">The IWebElement that is wrapped</param>
    public Table(OpenQA.Selenium.IWebElement table)
    {
        CheckType(table);
        WebElement = new WebElement(table);
    }

    /// <summary>
    ///     The table rows in the body section (zero based index)
    /// </summary>
    public IList<TableRow> BodyRows => TheRows(Section("tbody"));

    /// <summary>
    ///     The caption of the table
    /// </summary>
    public OpenQA.Selenium.IWebElement Caption => Section("caption");

    /// <summary>
    ///     The table rows in the footer section
    /// </summary>
    public IList<TableRow> FooterRows => TheRows(Section("tfoot"));

    /// <summary>
    ///     The table rows in the header section
    /// </summary>
    public IList<TableRow> HeaderRows => TheRows(Section("thead"));

    public bool NoTableEntriesDisplayed => WebElement.FindElements(By.CssSelector(".no-table-entries")).Count == 1;

    /// <summary>
    ///     The table rows in the entire table (any section)
    /// </summary>
    public IList<TableRow> Rows => TheRows(WebElement);

    public CheckBoxElement SelectAllRowsCheckbox => new(WebElement.FindElement(By.Name("toggleAllRowsSelected")));

    public IList<CheckBoxElement> SelectRowCheckboxes => WebElement.FindElements(By.Name("toggleRowSelected")).Select(x => new CheckBoxElement(x)).ToList();

    private static void CheckType(OpenQA.Selenium.IWebElement webElement)
    {
        if (!webElement.TagName.Equals(TagName))
        {
            throw new UnexpectedTagNameException(TagName, webElement.TagName);
        }
    }

    private static IList<TableRow> TheRows(ISearchContext parentElement)
    {
        var webElements = parentElement.FindElements(By.TagName("tr"));
        var list = new List<TableRow>(webElements.Count);
        list.AddRange(webElements.Select(webElement => new TableRow(webElement)));
        return list;
    }

    private IWebElement Section(string tagName)
    {
        return new WebElement(WebElement.FindElement(By.TagName(tagName)));
    }
}