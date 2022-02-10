using System.Linq;
using OpenQA.Selenium;

namespace Six.Test.Selenium.Extension.WebDriver.WebElements;

internal class CalendarPicker
{
    public CalendarPicker(By by)
    {
        DatePickerParent = TestDirector.WebDriver.FindElement(by);
    }

    public int Day
    {
        set
        {
            var day = DatePickerParent.FindElements(By.CssSelector("div.flatpickr-days>div.dayContainer span:not(.prevMonthDay)"))
                .First(e => new WebElement(e).Text.Equals(value.ToString()));
            day.Click();
        }
    }

    public int Month
    {
        set
        {
            var month = new WebElement(DatePickerParent.FindElement(By.CssSelector("div.flatpickr-month select"))).Selector();
            month.SelectByValue((value - 1).ToString()); // zero-based Jan=0
        }
    }

    public int Year
    {
        set
        {
            var year = new WebElement(DatePickerParent.FindElement(By.CssSelector("input.numInput.cur-year"))).TextField("number");
            year.TypeText(value.ToString());
        }
    }

    private OpenQA.Selenium.IWebElement DatePickerParent { get; }
}